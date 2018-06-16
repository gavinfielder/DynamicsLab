using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;


public class Motion3DFileManager : MonoBehaviour {

    //prefab references set in inspector
    public GameObject FileButtonPrefab;
    public Transform ScrollViewContent;
    public InputField filenameInput;
    public InputField nameInput;
    public InputField descriptionInput;
   

    //Set by a file being clicked (see Motion3DFileButton)
    public string selectedFilename;

    //contains all the buttons
    private List<Motion3DFileButton> files;

    //Used for initialization
    void Start()
    {
        files = new List<Motion3DFileButton>();
        RefreshFilesList();
    }

    //Refreshes the files list
    public void RefreshFilesList()
    {
        RemoveFileButtons();
        AddFileButtons();
    }

    //Reads the saved problem directory and populates the files list
    private void AddFileButtons()
    {
        //TODO: platform checks? this might only be windows
        string path = "Assets\\Motion3D\\SavedProblems\\";

        StreamReader fin;
        string[] filenames = Directory.GetFiles(path, "*.3dmotion");
        for (int i = 0; i < filenames.Length; i++)
        {
            fin = new StreamReader(filenames[i]);
            files.Add(Instantiate(FileButtonPrefab).GetComponent<Motion3DFileButton>());
            files[files.Count - 1].gameObject.SetActive(true);
            files[files.Count - 1].transform.SetParent(ScrollViewContent);
            files[files.Count - 1].Filename = stripPath(filenames[i]);
            files[files.Count - 1].Name = fin.ReadLine();
            files[files.Count - 1].Description = fin.ReadLine();
            fin.Close();
        }

    }

    //Removes all buttons
    private void RemoveFileButtons()
    {
        while (files.Count > 0)
        {
            //Remove from content to give safe place to wait for garbage collection
            files[0].transform.SetParent(this.transform);
            //Set button to inactive to hide it
            files[0].gameObject.SetActive(false);
            //Remove reference
            files.RemoveAt(0);
        }
    }

    // Deselects all buttons
    public void DeselectAll()
    {
        for (int i = 0; i < files.Count; i++)
        {
            files[i].Deselect();
        }
    }
 
    //Strips the leading path off a filename with a path, returns the filename
    private string stripPath(string filename_withpath)
    {
        //TODO: platform checks? this might be windows specific
        int i = filename_withpath.Length - 1;
        while (filename_withpath.Substring(i, 1) != "\\")
            i--;
        return filename_withpath.Substring(i+1);
    }

    //Strips the following .3dmotion file extension off of a filename
    private string stripFileExtension(string filename)
    {
        return filename.Substring(0, filename.Length - 9);
    }

    //Saves the file (this is a GUI handler)
    public void SaveButton_OnPress()
    {
       /* Gavin: to review
		string path = "Assets/Motion3D/SavedProblems/"+filenameInput.text.ToString()+".3dmotion";
		System.IO.File.WriteAllText(path,"");
    	StreamWriter writer = new StreamWriter(path, true);

		writer.WriteLine(nameInput.text.ToString());
		writer.WriteLine(descriptionInput.text.ToString());

		//Debug.Log( EquationX.text.ToString());

		writer.WriteLine( EquationX.text.ToString() );
		writer.WriteLine( EquationY.text.ToString() );
		writer.WriteLine( EquationZ.text.ToString() );

    	writer.Close();

    	RefreshFilesList();

    	return;
        */
    }

    /* Gavin: this was already defined in Motion3DSceneController
    //Loads the file
    public void LoadButton_OnPress(){

		string path = "Assets/Motion3D/SavedProblems/"+filenameInput.text.ToString()+".3dmotion";
		StreamReader reader = new StreamReader(path);

		string name = reader.ReadLine();
		string desc = reader.ReadLine();
		string x = reader.ReadLine();
		string y = reader.ReadLine();
		string z = reader.ReadLine();
		int order = 1;

		//Debug.Log(x+y+z);

		GameObject.Find("Motion3DConfigView").
			gameObject.GetComponent<Motion3DConfigGui>().
			Overwrite_Equations(x,y,z,order);

		reader.Close();

    }*/

    //Closes the menu (this is a GUI handler)
    public void CloseButton_OnPress()
    {
        gameObject.SetActive(false);
    }
}
