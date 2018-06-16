using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Motion3DFileButton : MonoBehaviour {

    //public references set in inspector
    public Text filenameText;
    public Text nameText;
    public Text descriptionText;
    public Button button;

    //Reference to the file manager
    static public Motion3DFileManager fileManager;

    //public properties
    public string Filename
    {
        get
        {
            return ensureFileExtension(filenameText.text);
        }
        set
        {
            filenameText.text = ensureFileExtension(value);
        }
    }
    public string Name
    {
        get
        {
            return nameText.text;
        }
        set
        {
            nameText.text = value;
        }
    }
    public string Description
    {
        get
        {
            return descriptionText.text;
        }
        set
        {
            descriptionText.text = value;
        }
    }

    //Used for initialization
    private void Start()
    {
        if (fileManager == null)
        {
            fileManager = GameObject.Find("FileManagerView").GetComponent<Motion3DFileManager>();
        }
    }
    
    //Selects/deselects (controls background color)
    public void Select()
    {
        //Set highlighted background color
        button.image.color = new Color(1.0f, 1.0f, 0f);
        //Set new selected filename in the filemanager
        fileManager.selectedFilename = Filename;
        //Populate the input fields in the file manager view
        fileManager.filenameInput.text = stripFileExtension(Filename);
        fileManager.nameInput.text = Name;
        fileManager.descriptionInput.text = Description;
    }
    public void Deselect()
    {
        //Set normal background color
        button.image.color = new Color(1.0f,1.0f,1.0f);
    }

    //Called when the button is clicked
    public void OnClick()
    {
        fileManager.DeselectAll(); //make sure no others are selected
        Select(); //select me
    }

    //Strips the following .3dmotion file extension off of a filename
    private string stripFileExtension(string filename)
    {
        return filename.Substring(0, filename.Length - 9);
    }

    //Ensures there is a .3dmotion file extension on a filename
    private string ensureFileExtension(string filename)
    {
        if (filename.Length < 10)
        {
            return filename + ".3dmotion";
        }
        else if (filename.Substring(filename.Length - 9) == ".3dmotion")
        {
            return filename;
        }
        else
        {
            return filename + ".3dmotion";
        }
    }
}
