using UnityEngine;
using UnityEngine.Video;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Linq;

using Application = UnityEngine.Application;

public class NetworkManager : MonoBehaviour
{

    GameObject joinb;
    GameObject exitb;
    GameObject fileb;
    GameObject imgb;
    GameObject pdfb;
    GameObject steuerung;
    bool steuerungboolean;
    bool PDFselected;
    bool PPselected;
    bool isAdmin;
    bool Videostop;
    GameObject plane;
    GameObject planePP;
    GameObject PPTaktiv;
    GameObject PDFaktiv;
    GameObject HMenu;
    GameObject NTB;
    GameObject NTBbtn;
    GameObject Warnhinweis;

    void Start()
    {
        joinb = GameObject.FindGameObjectWithTag("JoinButton");
        exitb = GameObject.FindGameObjectWithTag("ExitButton");
        fileb = GameObject.FindGameObjectWithTag("FileButton");
        imgb = GameObject.FindGameObjectWithTag("ImgButton");
        pdfb = GameObject.FindGameObjectWithTag("PDFButton");
        steuerung = GameObject.FindGameObjectWithTag("Steuerung");
        plane = GameObject.FindGameObjectWithTag("ImagePlane");
        planePP = GameObject.FindGameObjectWithTag("PresentationPlane");
        PPTaktiv = GameObject.FindGameObjectWithTag("PPTAktiv");
        PDFaktiv = GameObject.FindGameObjectWithTag("PDFAktiv");
        HMenu = GameObject.FindGameObjectWithTag("HMenu");
        NTB = GameObject.FindGameObjectWithTag("Nutzungsbedingung");
        NTBbtn = GameObject.FindGameObjectWithTag("NTBbtn");
        Warnhinweis = GameObject.FindGameObjectWithTag("Warnhinweis");

        NTBbtn.SetActive(false);
        HMenu.SetActive(false);
        PPTaktiv.SetActive(false);
        PDFaktiv.SetActive(false);
        joinb.SetActive(false);
        exitb.SetActive(false);
        fileb.SetActive(false);
        imgb.SetActive(false);
        pdfb.SetActive(false);
        steuerungboolean = false;
        PDFselected = false;
        PPselected = false;
        steuerung.SetActive(false);
        Warnhinweis.SetActive(false);
        Connect();
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("MeetingRoom");
        PhotonNetwork.autoJoinLobby=true;
    }
    

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
        NTBbtn.SetActive(true);
    }

    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
        isAdmin = true;
        NTBbtn.SetActive(true);

    }

    void OnJoinedRoom()
    {
        Debug.Log("joined room");
        joinb.SetActive(false);
        
    }

    public void SpawnMyPlayer()
    {
        joinb.SetActive(false);
        exitb.SetActive(true);
        HMenu.SetActive(true);
        Warnhinweis.SetActive(false);

        GameObject playerObject = (GameObject)PhotonNetwork.Instantiate("PlayerController", Vector3.one, Quaternion.identity, 0);
        ((MonoBehaviour)playerObject.GetComponent("FirstPersonController")).enabled = true;
        playerObject.GetComponent<AudioSource>().enabled = true;
        playerObject.GetComponent<CharacterController>().enabled = true;
        playerObject.GetComponentInChildren<Camera>().enabled = true;
    }

    public void closeApplication()
    {
       Application.Quit();
    }

    public void closeNTB()
    {
        NTB.SetActive(false);
        NTBbtn.SetActive(false);
        Warnhinweis.SetActive(true);
        joinb.SetActive(true);
        if (isAdmin)
        {
            fileb.SetActive(true);
            imgb.SetActive(true);
            pdfb.SetActive(true);
            isAdmin = false;
        }
        
    }

    public void openExplorerVideo()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        GameObject.FindGameObjectWithTag("VideoPlane").GetComponent<VideoPlayer>().url=paths[0];
        Videostop = true;
        
    }

    [PunRPC]
    void Video(string paths)
    {
        GameObject.FindGameObjectWithTag("Video").GetComponent<VideoPlayer>().url = paths;
        GameObject.FindGameObjectWithTag("VideoPlane").GetComponent<VideoPlayer>().Pause();
    }

    int k = 0;
    Texture2D[] textures2 = null;
    public void openExplorerImg()
    {
                      
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", true);
        FileInfo[] info = paths.Select(f => new FileInfo(f)).ToArray();
        textures2 = new Texture2D[info.Length];

        for (int i = 0; i < info.Length; i++)
        {
            MemoryStream dest = new MemoryStream();
            //Read from each Image File
            using (Stream source = info[i].OpenRead())
            {
                byte[] buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dest.Write(buffer, 0, bytesRead);
                }
            }
            byte[] imageBytes = dest.ToArray();
            //Create new Texture2D
            Texture2D tempTexture = new Texture2D(2, 2);
            //Load the Image Byte to Texture2D
            tempTexture.LoadImage(imageBytes);
            //Put the Texture2D to the Array
            textures2[i] = tempTexture;
        }

        planePP.GetComponent<MeshRenderer>().material.mainTexture = textures2[0];

     }
            
    int j = 0;
    Texture2D[] textures = null;
    public void openExplorerPDF()
    {       
        
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", true);
                
        FileInfo[] info = paths.Select(f => new FileInfo(f)).ToArray();
        textures = new Texture2D[info.Length];

         for (int i = 0; i < info.Length; i++)
         {
             MemoryStream dest = new MemoryStream();

             //Read from each Image File
             using (Stream source = info[i].OpenRead())
             {
                 byte[] buffer = new byte[2048];
                 int bytesRead;
                 while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                 {
                     dest.Write(buffer, 0, bytesRead);
                 }
             }
             byte[] imageBytes = dest.ToArray();
             //Create new Texture2D
             Texture2D tempTexture = new Texture2D(2, 2);
             //Load the Image Byte to Texture2D
             tempTexture.LoadImage(imageBytes);
             //Put the Texture2D to the Array
             textures[i] = tempTexture;
         }

        plane.GetComponent<MeshRenderer>().material.mainTexture = textures[0];
       

        }

    public Texture2D[] LoadIMG(string[] filePath)
    {
        Texture2D[] tex = null;
        byte[] fileData;

        for (int i = 0; i < filePath.Length; i++)
        {
            fileData = File.ReadAllBytes(filePath[i]);
            tex[i] = new Texture2D(2, 2);
            tex[i].LoadImage(fileData);
        }
        return tex;
    }

   
    void Update()
    {
        
        if (Input.GetKeyDown("h") && steuerungboolean == false)
        {
            steuerung.SetActive(true);
            steuerungboolean = true;
        }
        else if (Input.GetKeyDown("h") && steuerungboolean == true)
        {
            steuerung.SetActive(false);
            steuerungboolean = false;
        }

        if (Input.GetKeyDown("f") && PDFselected == false)
        {
            PDFselected = true;
            PDFaktiv.SetActive(true);
            PPselected = false;
            PPTaktiv.SetActive(false);

        }
        else if (PDFselected == true && Input.GetKeyDown("f"))
        {
            PDFselected = false;
            PPselected = false;
            PDFaktiv.SetActive(false);
            PPTaktiv.SetActive(false);

        }

        if (Input.GetKeyDown("p") && PPselected == false)
        {
            PPselected = true;
            PDFselected = false;
            PDFaktiv.SetActive(false);
            PPTaktiv.SetActive(true);
        }
        else if (PPselected == true && Input.GetKeyDown("p"))
        {
            PPselected = false;
            PDFselected = false;
            PDFaktiv.SetActive(false);
            PPTaktiv.SetActive(false);

        }

        if (Input.GetKeyDown("e"))
        {
            if (PDFselected == true)
            {
                if (j < textures.Length)
                {
                    j++;
                    plane.GetComponent<MeshRenderer>().material.mainTexture = textures[j];
                }
                else
                {
                    plane.GetComponent<MeshRenderer>().material.mainTexture = textures[0];
                }
            }
            else if (PPselected == true)
            {
                if (k < textures2.Length)
                {
                    k++;
                    planePP.GetComponent<MeshRenderer>().material.mainTexture = textures2[k];
                }
                else
                {
                    planePP.GetComponent<MeshRenderer>().material.mainTexture = textures2[0];
                }

            }
        }

        if(Input.GetKeyDown("r"))
        {
            if(Videostop)
            { 
            GameObject.FindGameObjectWithTag("VideoPlane").GetComponent<VideoPlayer>().Play();
                Videostop = false;
            }
            else if(Videostop == false)
            {
                GameObject.FindGameObjectWithTag("VideoPlane").GetComponent<VideoPlayer>().Pause();
                Videostop = true;
            }
        }

        if (Input.GetKeyDown("q"))
        {
            if (PDFselected == true)
            {
                if (j > -1)
                {
                    j--;
                    plane.GetComponent<MeshRenderer>().material.mainTexture = textures[j];
                }
                else
                {
                    j = textures.Length;
                    plane.GetComponent<MeshRenderer>().material.mainTexture = textures[j];
                }

            }
            else if (PPselected == true)
            {
                if (k > -1)
                {
                    k--;
                    planePP.GetComponent<MeshRenderer>().material.mainTexture = textures2[k];
                }
                else
                {
                    k = textures2.Length;
                    planePP.GetComponent<MeshRenderer>().material.mainTexture = textures2[k];
                }
            }
        }

    }


}
