using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using Cognex.VisionPro.ImageFile;
using System.IO;

namespace UI
{
  public class ImageSaveQueue
  {
    class ImageAndPath
    {
      public ICogImage CogImage;
      public Image SystemImage;
      public bool SavedToDisk;
      private string m_savedToDiskPath;
      public UInt64 PartID;
      object m_sync;

      public String SavePath;
      public ImageAndPath(String savePath, ICogImage cogImage, UInt64 partID)
      {
        SavePath = savePath;
        CogImage = cogImage;
        SystemImage = null;
        SavedToDisk = false;
        PartID = partID;
        m_sync = new object();
      }

      public ImageAndPath(String savePath, Image sysImage, UInt64 partID)
      {
        SavePath = savePath;
        SystemImage = sysImage;
        CogImage = null;
        SavedToDisk = false;
        PartID = partID;
        m_sync = new object();
      }

      public String Rename()
      {
        return Rename(PartID);
      }

      public String Rename(UInt64 partID)
      {
        lock (m_sync)
        {
          PartID = partID;
          String partName = ImageSaveQueue.gOnly.GetPartName(PartID);
          String newSavePath = String.Format(SavePath, partName);
          if (PartID > 0 && partName != null)
          {
            if (SavedToDisk && m_savedToDiskPath != newSavePath)
            {
              // Physically move the file on disk
              try
              {
                String emptyDirectory = Path.GetDirectoryName(m_savedToDiskPath);
                String parentDirectory = Path.GetDirectoryName(newSavePath);
                Directory.CreateDirectory(parentDirectory);
                File.Move(m_savedToDiskPath, newSavePath);
                m_savedToDiskPath = newSavePath;
                return emptyDirectory;
              }
              catch (Exception ex)
              {
               // MessageManager.gOnly.Alarm("Unable to move saved image!", AlarmType.Unknown, ex);
              }
            }
          }
        }

        return null;

      }

      public void SaveToDisk()
      {
        lock (m_sync)
        {


          String partName = ImageSaveQueue.gOnly.GetPartName(PartID);
          if (partName != null && PartID > 0)
          {
            m_savedToDiskPath = String.Format(SavePath, partName);
          }
          else
          {
            m_savedToDiskPath = SavePath;
          }

          String parentDirectory = Path.GetDirectoryName(m_savedToDiskPath);
          Directory.CreateDirectory(parentDirectory);

          if (CogImage != null)
          {



            try
            {
              if (SavePath.EndsWith(".png"))
              {
                CogImageFilePNG file = new CogImageFilePNG();
                file.Open(m_savedToDiskPath, CogImageFileModeConstants.Write);
                file.Append(CogImage);
                file.Close();
              } 
              else if (SavePath.EndsWith(".bmp"))
              {
                CogImageFileBMP file = new CogImageFileBMP();
                file.Open(m_savedToDiskPath, CogImageFileModeConstants.Write);
                file.Append(CogImage);
                file.Close();
              }
              else if (SavePath.EndsWith(".tif") || SavePath.EndsWith(".tiff"))
              {
                CogImageFileTIFF file = new CogImageFileTIFF();
                file.Open(m_savedToDiskPath, CogImageFileModeConstants.Write);
                file.Append(CogImage);
                file.Close();
              }
              else if (SavePath.EndsWith(".vri"))
              {
                CogSerializer.SaveObjectToFile(CogImage, m_savedToDiskPath);
              }
              else if (SavePath.EndsWith(".cdb"))
              {
                CogImageFileCDB file = new CogImageFileCDB();
                file.Open(m_savedToDiskPath, CogImageFileModeConstants.Write);
                file.Append(CogImage);
                file.Close();
              }
              else
              {
                CogImageFileBMP file = new CogImageFileBMP();
                file.Open(m_savedToDiskPath, CogImageFileModeConstants.Write);
                file.Append(CogImage);
                file.Close();
              }
              CogImage = null;
              SavedToDisk = true;
            }
            catch (Exception ex)
            {
              //MessageManager.gOnly.Alarm(String.Format("Failed to save system image to {0}", m_savedToDiskPath), AlarmType.Unknown, ex);
            }
          }
          else if (SystemImage != null)
          {
            try
            {
              SystemImage.Save(m_savedToDiskPath, System.Drawing.Imaging.ImageFormat.Jpeg);
              SystemImage = null;
              SavedToDisk = true;
            }
            catch (Exception ex)
            {
              //MessageManager.gOnly.Alarm(String.Format("Failed to save system image to {0}", m_savedToDiskPath), AlarmType.Unknown, ex);
            }
          }
        }
      }
    }

    #region Singleton Stuff
    private static ImageSaveQueue m_this = null;
    public static void gInit(int queueSize = 32)
    {
      if (m_this == null)
        m_this = new ImageSaveQueue(queueSize);
      if (m_this == null)
        throw new Exception("Can not create ImageSaveQueue object.");
      m_this.Init();
    }
    public static void gShutDown()
    {
      if (m_this != null)
        m_this.Shutdown();
    }
    public static ImageSaveQueue gOnly
    {
      get { return m_this; }
    }

    ~ImageSaveQueue()
    {
    }
    #endregion

    int m_imageQueueSize;

    bool m_exit;
    Thread m_thread;
    SemaphoreSlim m_semaphore;
    Object m_lock;
    Queue<ImageAndPath> m_imageQueue;
    List<ImageAndPath> m_activeImageRecords;

    DateTime m_lastCleanup;
    UInt64 m_partID = 1;
    Dictionary<UInt64, String> m_partNames;
    Queue<UInt64> m_activePartIDs;

    public ImageSaveQueue(int queueSize = 32)
    {
      m_imageQueueSize = queueSize;
      m_exit = false;
      m_thread = new Thread(new ThreadStart(workerThread));
      m_semaphore = new SemaphoreSlim(0);
      m_lock = new Object();
      m_imageQueue = new Queue<ImageAndPath>();
      m_activeImageRecords = new List<ImageAndPath>();
      m_lastCleanup = DateTime.MinValue;
      m_partID = 1;
      m_partNames = new Dictionary<ulong, string>();
      m_activePartIDs = new Queue<ulong>();
      m_thread.Start();
    }

    public void Init()
    {
    }

    public UInt64 NewPart()
    {
      lock (m_lock)
      {
        UInt64 newPartID = m_partID;
        m_partID++;

        if (!m_partNames.ContainsKey(newPartID))
          m_partNames.Add(newPartID, String.Format("{0}_UnknownPart_{1}", DateTime.Now.ToString("HHmmssfff"), newPartID));

        m_activePartIDs.Enqueue(newPartID);

        // Go through the dictionary and remove old items
        while (m_activePartIDs.Count > 100)
        {
          UInt64 expiredID = m_activePartIDs.Dequeue();
          if (m_partNames.ContainsKey(expiredID))
            m_partNames.Remove(expiredID);
        }

        return newPartID;
      }
    }

    public UInt64 NewPart(String partName)
    {
      lock (m_lock)
      {
        UInt64 newPartID = m_partID;
        m_partID++;

        if (!m_partNames.ContainsKey(newPartID))
          m_partNames.Add(newPartID, String.Format("{0}_{1}", DateTime.Now.ToString("HHmmssfff"), partName));

        m_activePartIDs.Enqueue(newPartID);

        // Go through the dictionary and remove old items
        while (m_activePartIDs.Count > 100)
        {
          UInt64 expiredID = m_activePartIDs.Dequeue();
          if (m_partNames.ContainsKey(expiredID))
            m_partNames.Remove(expiredID);
        }

        return newPartID;
      }
    }

    public void SetPartName(String newPartName, params UInt64[] oldPartIDs)
    {
      if (oldPartIDs == null || oldPartIDs.Length < 1)
        return;

      lock (m_lock)
      {

        UInt64 newPartID = 0;

        foreach (UInt64 partID in oldPartIDs)
        {
          if (partID == 0)
            continue;

          if (newPartID == 0)
          {
            newPartID = partID;
            SetPartName(newPartName, newPartID, partID);
          }
          else
          {
            MergeParts(newPartID, partID);
          }

        }
      }
    }

    public void SetPartName(String newPartName, params String[] oldPartNames)
    {
      if (oldPartNames == null || oldPartNames.Length < 1)
        return;

      lock (m_lock)
      {
        UInt64 newPartID = 0;

        List<UInt64> keys = new List<UInt64>(m_partNames.Keys);

        foreach (UInt64 partID in keys)
        {

          // The first 10 characters of the part name is just the timestamp and underscore.
          // Ignore those.

          if (partID == 0 || m_partNames[partID] == null || m_partNames[partID].Length < 11)
            continue;

          String oldPartName = m_partNames[partID].Substring(10);
          if (oldPartNames.Contains(oldPartName))
          {
            if (newPartID == 0)
            {
              newPartID = partID;
              SetPartName(newPartName, newPartID, partID);
            }
            else
            {
              MergeParts(newPartID, partID);
            }

          }
        }

        
      }
    }

    public void SetPartName(String newPartName, UInt64 partID)
    {
      SetPartName(newPartName, partID, partID);
    }

    private void MergeParts(UInt64 newPartID, UInt64 oldPartID)
    {
      // Go through all the parts in the queue and leftovers using this ID and rename them
      List<String> emptyDirectories = new List<string>();
      foreach (ImageAndPath iap in m_activeImageRecords)
      {
        if (iap.PartID == oldPartID)
        {
          String emptyDirectory = iap.Rename(newPartID);
          emptyDirectories.Add(emptyDirectory);
        }
      }

      // Delete any directories now empty
      foreach (String directory in emptyDirectories)
      {
        if (directory != null && Directory.Exists(directory) && IsDirectoryEmpty(directory))
        {
          Directory.Delete(directory, true);
        }
      }

      m_partNames.Remove(oldPartID);

      // Go through the dictionary and remove old items
      while (m_activePartIDs.Count > 1000)
      {
        UInt64 expiredID = m_activePartIDs.Dequeue();
        if (m_partNames.ContainsKey(expiredID))
          m_partNames.Remove(expiredID);
      }
    }

    private void SetPartName(String newPartName, UInt64 newPartID, UInt64 oldPartID)
    {
      newPartName = String.Format("{0}_{1}", DateTime.Now.ToString("HHmmssfff"), newPartName);
      lock (m_lock)
      {
        // Set the name in the dictionary
        if (!m_partNames.ContainsKey(oldPartID))
        {
          m_partNames.Add(oldPartID, newPartName);
        }
        else
        {
          m_partNames[oldPartID] = newPartName;
        }

        // Go through all the parts in the queue and leftovers using this ID and rename them
        List<String> emptyDirectories = new List<string>();
        foreach (ImageAndPath iap in m_activeImageRecords)
        {
          if (iap.PartID == oldPartID)
          {
            String emptyDirectory = iap.Rename(newPartID);
            emptyDirectories.Add(emptyDirectory);
          }
        }

        // Delete any directories now empty
        foreach (String directory in emptyDirectories)
        {
          if (directory != null && Directory.Exists(directory) && IsDirectoryEmpty(directory))
          {
            Directory.Delete(directory, true);
          }
        }

        // Go through the dictionary and remove old items
        while (m_activePartIDs.Count > 1000)
        {
          UInt64 expiredID = m_activePartIDs.Dequeue();
          if (m_partNames.ContainsKey(expiredID))
            m_partNames.Remove(expiredID);
        }
      }
    }

    public String GetPartName(UInt64 partID)
    {
      lock (m_lock)
      {
        if (m_partNames.ContainsKey(partID))
          return m_partNames[partID];
        else
          return null;
      }
    }

    public bool AddToQueue(String savePath, ICogImage image, UInt64 partID = 0)
    {
      lock (m_lock)
      {
        if (m_imageQueue.Count < m_imageQueueSize)
        {
          ImageAndPath newQueuedImage = new ImageAndPath(savePath, image, partID);
          m_imageQueue.Enqueue(newQueuedImage);
          if (partID > 0)
            m_activeImageRecords.Add(newQueuedImage);
          while (m_activeImageRecords.Count > 255)
          {
            m_activeImageRecords.RemoveAt(0);
          }

          m_semaphore.Release();
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    public bool AddToQueue(String savePath, Image image, UInt64 partID = 0)
    {
      lock (m_lock)
      {
        if (m_imageQueue.Count < m_imageQueueSize)
        {
          ImageAndPath newQueuedImage = new ImageAndPath(savePath, image, partID);
          m_imageQueue.Enqueue(newQueuedImage);
          if (partID > 0)
            m_activeImageRecords.Add(newQueuedImage);
          while (m_activeImageRecords.Count > 255)
          {
            m_activeImageRecords.RemoveAt(0);
          }
          m_semaphore.Release();
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    private void workerThread()
    {
      while (!m_exit)
      {
        if (m_semaphore.Wait(200))
        {
          ImageAndPath imageToSave = null;
          lock (m_lock)
          {
            if (m_imageQueue.Count > 0)
              imageToSave = m_imageQueue.Dequeue();
          }

          if (imageToSave != null)
          {
            try
            {
              imageToSave.SaveToDisk();
            }
            catch (Exception ex)
            {
              //MessageManager.gOnly.Alarm(String.Format("Failed to save system image to {0}", imageToSave.SavePath), AlarmType.Unknown, ex);
            }
          }
        }
      }
    }

    public bool IsDirectoryEmpty(string path)
    {
      return !Directory.EnumerateFileSystemEntries(path).Any();
    }

    private void deleteEmptyFolders(string startLocation)
    {
      foreach (var directory in Directory.GetDirectories(startLocation))
      {
        deleteEmptyFolders(directory);
        if (Directory.GetFiles(directory).Length == 0 &&
            Directory.GetDirectories(directory).Length == 0)
        {
          try
          {
            Directory.Delete(directory, false);
          }
          catch
          {
            //MessageManager.gOnly.Info(String.Format(LocalizationService.gOnly.GetLocalizedString("Could not delete folder {0} during image cleanup."), startLocation));
          }
        }
      }
    }

    public void Shutdown()
    {
      if (m_thread != null)
      {
        m_exit = true;
        m_thread.Join();
        m_thread = null;
      }
    }

  }
}
