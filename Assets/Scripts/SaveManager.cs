using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using SFB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    private string folderPath;

    private string filePath;
    private SaveData saveData;
    private string fileName;

    private Button selectFolderButton;

    private static SaveManager instance;

    private void Awake() {
        if (instance == null)
        {
            folderPath = Application.dataPath + "/SaveData/FirstData";
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        FirstSave();        
    }
    // Start is called before the first frame update
    void Start()
    {
        selectFolderButton = GameObject.Find("Canvas/Setting/SelectFileButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public string GetFolderPath(){
        return folderPath;
    }

    public void ChangeFolder(string folderPath){

        //保存先のフォルダを変更
        this.folderPath = folderPath;
        //フォルダを変更
        Save();
        SceneManager.LoadScene("TrainingScene");
        
    }

    



    public void FirstSave(){
        fileName =  "SaveManager" + ".json";
        filePath = Application.dataPath + "/SaveData/"  + fileName;
        Debug.Log(filePath);
        saveData = new SaveData();
        // ファイルがないとき、ファイル作成
        if (!File.Exists(filePath)) {
            Save();
        }

        // ファイルを読み込んでdataに格納
        saveData = Load(filePath);

        folderPath = saveData.folderPath;
    }

    //-------------------------------------------------------------------
    // jsonとしてデータを保存
    public void Save()
    {
        saveData.folderPath = folderPath;
        Debug.Log(folderPath + "に変更");
        string json = JsonUtility.ToJson(saveData,true);
        StreamWriter wr = new StreamWriter(filePath, false);
        wr.WriteLine(json);
        wr.Flush();                                  
        wr.Close();
    }

    // jsonファイル読み込み
    SaveData Load(string path)
    {
        StreamReader rd = new StreamReader(path);
        string json = rd.ReadToEnd();
        rd.Close();
                                                                
        return JsonUtility.FromJson<SaveData>(json);
    }

    [System.Serializable]
    public class SaveData{
        public string folderPath;
    }
    
}
