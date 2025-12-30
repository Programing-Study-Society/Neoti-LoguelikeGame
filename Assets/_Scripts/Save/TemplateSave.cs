using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class TemplateSave : MonoBehaviour
{
    //※SaveDataクラスは別途作成してください
    [HideInInspector] public TemplateSaveData data;     // json用のデータ保存クラス
    string filePath;                            // jsonファイルのパス
    string fileName = "SaveData.json";          // jsonファイル名
    string metaFileName = "SaveData.json.meta";

    // json形式でデータを保存
    void jsonSave(TemplateSaveData data)
    {
        InSaveData();
        string json = JsonUtility.ToJson(data);                 // json形式に変換
        StreamWriter wr = new StreamWriter(filePath, false);    // ファイルを書き込みモードで開く
        wr.Write(json);                                         // jsonデータを書き込み
        wr.Flush();
        wr.Close();                                             // ファイルを閉じる
    }

    // jsonファイルを読み込む
    TemplateSaveData Load(string path)
    {
        StreamReader rd = new StreamReader(path);               // ファイルを読み込みモードで開く
        string json = rd.ReadToEnd();                           // ファイルの中身を全て読み込む
        rd.Close();                                             // ファイルを閉じる

        return JsonUtility.FromJson<TemplateSaveData>(json);            // jsonをTemplateSaveDataに変換
    }

    void Awake()
    {
        filePath = Application.dataPath + "/" + fileName;
    }

    // スタート時にJsonファイルから値を読み込む
    public void StartRoad()
    {
        // パスを設定
        filePath = Application.dataPath + "/" + fileName;

        // ファイルが存在しない場合は新規作成
        if (!File.Exists(filePath))
        {
            jsonSave(data);
        }

        // ファイルから data に読み込み
        data = Load(filePath);

        // 読み込んだ値を GlobalValue に反映
        InGlobalValue();
    }

    // セーブ処理
    public void ClickSave()
    {
        // ゲームの状態を保存
        jsonSave(data);
        Debug.Log(filePath);
    }

    // セーブデータを削除して最初から
    public void ClickReStart()
    {
        JsonFileDelete();
        endGame();
    }

    // ゲーム終了
    public void endGame()
    {
        ClickSave();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタでの再生停止
#else
        Application.Quit(); // ビルド後のゲーム終了
#endif
    }

    // Jsonファイル削除
    void JsonFileDelete()
    {
        filePath = Application.dataPath + "/" + fileName;

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        filePath = Application.dataPath + "/" + metaFileName;

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // シーン再読み込み
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // GlovalValueのstaticの値を SaveData に保存
    public void InSaveData()
    {
        //savedata用のファイル値 = 保存したい値
        //例：data.Difficulty = GlovalValue.Difficulty;
        
        //音声ボリューム保存
        KeepVolume Volume = GetComponent<KeepVolume>();
        data.bgmVolume = Volume.bgmVolume;
        data.seVolume = Volume.seVolume;

        //スキルポイント関連
        SkillManeger skill = GetComponent<SkillManeger>();
        data.AttackSkillPoint = skill.AttackSkillPoint;
        data.DefenseSkillPoint = skill.DefenseSkillPoint;
        data.HpSkillPoint = skill.HpSkillPoint;

    }

    // SaveDataの値をゲームに反映
    public void InGlobalValue()
    {
        //保存したい値 = savedata用のファイル値
        //例：GlovalValue.Difficulty = data.Difficulty;
        //音声ボリューム反映
        KeepVolume Volume = GetComponent<KeepVolume>();
        Volume.bgmVolume = data.bgmVolume;
        Volume.seVolume = data.seVolume;

        //スキルポイント関連
        SkillManeger skill = GetComponent<SkillManeger>();
        skill.AttackSkillPoint = data.AttackSkillPoint;
        skill.DefenseSkillPoint = data.DefenseSkillPoint;   
        skill.HpSkillPoint = data.HpSkillPoint;
    }
}
