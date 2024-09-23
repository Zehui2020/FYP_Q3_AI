//----------------------------------------------------------------------
// PlayerWallet
//
// プレイヤーの所持金を管理するクラス
//
// Data: 2024/08/30
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.IO;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    // プレイヤーの所持金
    public static PlayerWallet Instance { get; private set; }

    // ゲームが開始されたときのみResourcesのjsonファイルから読み込む
    [Header("JSON File Name in Resources / Resourcesフォルダ内のJSONファイル名")]
    public string m_jsonFileName = "money";

    // 現在のプレイヤーの所持金
    private int m_currentMoney;

    void Start()
    {
        // 保存先確認用
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }

    void Awake()
    {
        // シングルトンパターンの実装: インスタンスが設定されていない場合の処理
        if (Instance == null)
        {
            // インスタンスとしてオブジェクトを設定する
            Instance = this;
            // シーンが切り替わってもオブジェクトを破壊しない
            DontDestroyOnLoad(gameObject);
            // 起動時にJSONから所持金を読み込む
            LoadMoney();
        }
        else
        {
            // 重複するインスタンスを破壊する
            Destroy(gameObject);
        }
    }

    // プレイヤーの所持金を取得する処理
    public int GetMoney()
    {
        // プレイヤーの現在の所持金を取得する
        return m_currentMoney;
    }

    // プレイヤーの所持金を追加する処理
    public void AddMoney(int amount)
    {
        // プレイヤーの所持金を追加した後に保存する
        m_currentMoney += amount;
        // プレイヤーの所持金を保存する
        SaveMoney();
    }

    // プレイヤーの所持金を使用する処理
    public bool SpendMoney(int amount)
    {
        // プレイヤーの所持金が購入額以上の場合の処理
        if (m_currentMoney >= amount)
        {
            // プレイヤーの所持金から指定された額を減らす
            m_currentMoney -= amount;
            // プレイヤーの所持金を保存する
            SaveMoney();
            // 購入を成功させる
            return true;
        }
        // プレイヤーの所持金が購入額未満の場合の処理
        else
        {
            // 購入を失敗させる
            return false;
        }
    }

    // プレイヤーの所持金を保存する処理
    private void SaveMoney()
    {
        // 所持金を保存するためのデータクラスのインスタンスを作成し、現在の所持金を設定する
        var data = new PlayerWalletData { money = m_currentMoney };

        // データクラスをJSON形式の文字列に変換する
        string json = JsonUtility.ToJson(data);

        // JSONデータを保存するファイルのパスを決定する
        string path = Path.Combine(Application.persistentDataPath, m_jsonFileName + ".json");

        // JSONデータをファイルに書き込む
        File.WriteAllText(path, json);
    }

    // プレイヤーの所持金を読み込む処理
    private void LoadMoney()
    {
        // 所持金データを保存するファイルのパスを決定する
        string path = Path.Combine(Application.persistentDataPath, m_jsonFileName + ".json");

        // 指定したパスにファイルが存在する場合の処理
        if (File.Exists(path))
        {
            // ファイルからJSONデータを読み込む
            string json = File.ReadAllText(path);

            // JSONデータをプレイヤーの所持金データに変換する
            PlayerWalletData data = JsonUtility.FromJson<PlayerWalletData>(json);

            // 読み込んだ所持金データを現在の所持金として設定する
            m_currentMoney = data.money;
        }
        // ファイルが存在しない場合の処理
        else
        {
            // デフォルトの所持金（10000G）を設定する
            m_currentMoney = 10000;
        }
    }
}

// プレイヤーの所持金を保存するためのデータクラス
[System.Serializable]
public class PlayerWalletData
{
    public int money;
}
