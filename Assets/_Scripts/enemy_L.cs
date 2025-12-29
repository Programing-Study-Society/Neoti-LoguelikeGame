using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のタイプ
/// </summary>
public enum EnemyType
{
    Common,      // 雑魚（全惑星に出現）
    MidBoss,     // 中盤ボス（惑星固有）
    Boss,        // ボス（惑星固有）
    FinalBoss    // 最終ボス
}

/// <summary>
/// 敵データ管理クラス
/// IDと敵情報を紐づけるデータファイル
/// item.csと同じ構造で、IDから敵情報を取得可能
/// </summary>
public class enemy_L : MonoBehaviour
{
    /// <summary>
    /// 敵データのリスト
    /// Key: 敵ID (0-17, 全18体)
    /// Value: [MAXHP, ATK, DEF, 持続ダメージ, ダメージダウン, ダメージアップ, 防御力ダウン, 防御力アップ]
    /// 3つごとに区切り、3個目がボス（HP300固定、最終ボスは500）
    /// </summary>
    public Dictionary<int, List<int>> enemy_list = new Dictionary<int, List<int>>()
    {
        // 全惑星共通（雑魚：序盤の敵）
        {0, new List<int>() {60, 21, 5, 0, 0, 0, 0, 0}},   // Claw_Loader (雑魚)
        {1, new List<int>() {50, 18, 4, 0, 0, 0, 0, 0}},   // Omni-Roid (雑魚)
        
        // Volcano_Planet (3体：中盤×2 + ボス×1)
        {2, new List<int>() {120, 20, 8, 0, 0, 0, 0, 0}},   // Welder (中盤)
        {3, new List<int>() {100, 25, 6, 0, 0, 0, 0, 0}},   // Fire_Ant (中盤)
        {4, new List<int>() {300, 28, 18, 0, 0, 0, 0, 0}},  // Typhon (ボス)
        
        // Ice_Planet (3体：中盤×2 + ボス×1)
        {5, new List<int>() {120, 18, 10, 0, 0, 0, 0, 0}},  // Prospector (中盤)
        {6, new List<int>() {100, 22, 8, 0, 0, 0, 0, 0}},   // Cable_Rigger (中盤)
        {7, new List<int>() {300, 25, 20, 0, 0, 0, 0, 0}},  // Boreas (ボス)
        
        // Forest_Planet (3体：中盤×2 + ボス×1)
        {8, new List<int>() {120, 24, 7, 0, 0, 0, 0, 0}},   // Strider (中盤)
        {9, new List<int>() {100, 20, 9, 0, 0, 0, 0, 0}},   // Timber_Jack (中盤)
        {10, new List<int>() {300, 26, 20, 0, 0, 0, 0, 0}},  // Briareus (ボス)
        
        // Old_Empire_Planet (3体：中盤×2 + ボス×1)
        {11, new List<int>() {120, 19, 10, 0, 0, 0, 0, 0}},  // Enforcer (中盤)
        {12, new List<int>() {100, 17, 12, 0, 0, 0, 0, 0}}, // Juggernaut (中盤)
        {13, new List<int>() {300, 30, 25, 0, 0, 0, 0, 0}}, // Talos_Zero (ボス)
        
        // Desert_Planet (3体：中盤×2 + ボス×1)
        {14, new List<int>() {120, 21, 9, 0, 0, 0, 0, 0}},  // Vulture (中盤)
        {15, new List<int>() {100, 19, 8, 0, 0, 0, 0, 0}},  // Louver_Ray (中盤)
        {16, new List<int>() {300, 28, 22, 0, 0, 0, 0, 0}}, // Seth_Gigas (ボス)
        
        // 最終ボス (未実装)
        {17, new List<int>() {1000, 35, 30, 0, 0, 0, 0, 0}}  // 最終ボス (未実装)
    };

    /// <summary>
    /// 敵IDと名前の対応表
    /// </summary>
    public Dictionary<int, string> enemy_name_list = new Dictionary<int, string>()
    {
        {0, "クローローダー"},
        {1, "オムニロイド"},
        {2, "ウェルダー"},
        {3, "ファイアーアント"},
        {4, "タイフォン"},
        {5, "プロスペクター"},
        {6, "ケーブルリガー"},
        {7, "ボレアス"},
        {8, "ストライダー"},
        {9, "ティンバージャック"},
        {10, "ブリアレウス"},
        {11, "エンフォーサー"},
        {12, "ジャガーノート"},
        {13, "タロスゼロ"},
        {14, "バルチャー"},       // Vulture (Desert_Planet 中盤)
        {15, "ルーバーレイ"},     // Louver_Ray (Desert_Planet 中盤)
        {16, "セト・ギガス"},     // Seth_Gigas (Desert_Planet ボス)
        {17, "バリアント"}         // 最終ボス (未実装)
    };

    /// <summary>
    /// 敵IDと画像ファイル名の対応表
    /// Resources/enemies/ フォルダから読み込む際に使用
    /// </summary>
    public Dictionary<int, string> enemy_image_map = new Dictionary<int, string>()
    {
        {0, "Claw_Loader"},
        {1, "Omni-Roid"},
        {2, "Welder"},
        {3, "Fire_Ant"},
        {4, "Typhon"},
        {5, "Prospector"},
        {6, "Cable_Rigger"},
        {7, "Boreas"},
        {8, "Strider"},
        {9, "Timber_Jack"},
        {10, "Briareus"},
        {11, "Enforcer"},
        {12, "Juggernaut"},
        {13, "Talos_Zero"},
        {14, "Vulture"},      // Vulture (Desert_Planet 中盤)
        {15, "Louver_Ray"},   // Louver_Ray (Desert_Planet 中盤)
        {16, "Seth_Gigas"},   // Seth_Gigas (Desert_Planet ボス)
        {17, "Variant"}        // Variant (最終ボス)
    };

    /// <summary>
    /// 敵IDと敵タイプの対応表
    /// 3つごとに区切り、3個目がボス
    /// </summary>
    public Dictionary<int, EnemyType> enemy_type_map = new Dictionary<int, EnemyType>()
    {
        {0, EnemyType.Common},    // Claw_Loader (全惑星 雑魚)
        {1, EnemyType.Common},    // Omni-Roid (全惑星 雑魚)
        {2, EnemyType.MidBoss},   // Welder (Volcano_Planet 中盤)
        {3, EnemyType.MidBoss},   // Fire_Ant (Volcano_Planet 中盤)
        {4, EnemyType.Boss},      // Typhon (Volcano_Planet ボス)
        {5, EnemyType.MidBoss},   // Prospector (Ice_Planet 中盤)
        {6, EnemyType.MidBoss},   // Cable_Rigger (Ice_Planet 中盤)
        {7, EnemyType.Boss},      // Boreas (Ice_Planet ボス)
        {8, EnemyType.MidBoss},   // Strider (Forest_Planet 中盤)
        {9, EnemyType.MidBoss},   // Timber_Jack (Forest_Planet 中盤)
        {10, EnemyType.Boss},     // Briareus (Forest_Planet ボス)
        {11, EnemyType.MidBoss},  // Enforcer (Old_Empire_Planet 中盤)
        {12, EnemyType.MidBoss},  // Juggernaut (Old_Empire_Planet 中盤)
        {13, EnemyType.Boss},     // Talos_Zero (Old_Empire_Planet ボス)
        {14, EnemyType.MidBoss},  // Vulture (Desert_Planet 中盤)
        {15, EnemyType.MidBoss},  // Louver_Ray (Desert_Planet 中盤)
        {16, EnemyType.Boss},     // Seth_Gigas (Desert_Planet ボス)
        {17, EnemyType.FinalBoss} // 最終ボス (未実装)
    };

    /// <summary>
    /// 敵IDと出現惑星の対応表
    /// "All"は全惑星に出現（雑魚用）
    /// BattleScreen.PlanetTypeの文字列と一致させる
    /// </summary>
    public Dictionary<int, string> enemy_planet_map = new Dictionary<int, string>()
    {
        {0, "All"},               // Claw_Loader (全惑星)
        {1, "All"},               // Omni-Roid (全惑星)
        {2, "Volcano_Planet"},     // Welder
        {3, "Volcano_Planet"},     // Fire_Ant
        {4, "Volcano_Planet"},     // Typhon
        {5, "Ice_Planet"},         // Prospector
        {6, "Ice_Planet"},         // Cable_Rigger
        {7, "Ice_Planet"},         // Boreas
        {8, "Forest_Planet"},      // Strider
        {9, "Forest_Planet"},      // Timber_Jack
        {10, "Forest_Planet"},     // Briareus
        {11, "Old_Empire_Planet"}, // Enforcer
        {12, "Old_Empire_Planet"}, // Juggernaut
        {13, "Old_Empire_Planet"}, // Talos_Zero
        {14, "Desert_Planet"},     // Vulture (Desert_Planet 中盤)
        {15, "Desert_Planet"},     // Louver_Ray (Desert_Planet 中盤)
        {16, "Desert_Planet"},     // Seth_Gigas (Desert_Planet ボス)
        {17, "None"}               // 最終ボス (全ステージクリア後)
    };

    // 以下は後方互換性のため残す（個別の敵インスタンス用）
    [Header("個別の敵インスタンス用（プレハブで使用）")]
    [Tooltip("敵ID (0-17): データファイルから自動的にステータスを読み込む")]
    public int enemyId = -1;  // 敵ID（-1は未設定）
    
    public int MAXHP = 0;  // 最大HP
    public int HP = 0;     // リアルタイムHP
    public int ATK = 0;    // 攻撃力
    public int DEF = 0;    // 防御力
    public List<int> BUFF = new List<int>()
    {
        0, // 持続ダメージ(デバフ)
        0, // ダメージダウン(デバフ)
        0, // ダメージアップ(バフ)
        0, // 防御力ダウン(デバフ)
        0  // 防御力アップ(バフ)
    };
    
    [Header("データファイル参照")]
    [Tooltip("敵データファイル（enemy_Lコンポーネント）。nullの場合はFindObjectOfTypeで自動検索")]
    public enemy_L dataFile;  // データファイルへの参照
    
    //------------------------------------------------------------------------------------------
    //☆敵の情報を取得するメソッド群これらのメソッドを呼び出すだけで必要なデータ取ってこれるはずです。☆
    //------------------------------------------------------------------------------------------
    
    /// <summary>
    /// 敵IDを設定してデータファイルからステータスを読み込む
    /// プレハブからInstantiateした後に呼び出す
    /// </summary>
    /// <param name="id">敵ID (0-17)</param>
    public void LoadFromDataFile(int id)
    {
        enemyId = id;
        LoadStatsFromDataFile();
    }
    
    /// <summary>
    /// データファイルから現在のenemyIdのステータスを読み込む
    /// </summary>
    public void LoadStatsFromDataFile()
    {
        if (enemyId < 0)
        {
            Debug.LogWarning("enemy_L: enemyIdが設定されていません");
            return;
        }
        
        // データファイルを取得（参照がなければ自動検索）
        enemy_L data = dataFile;
        if (data == null)
        {
            data = FindObjectOfType<enemy_L>();
        }
        
        if (data == null)
        {
            Debug.LogError("enemy_L: 敵データファイル（enemy_Lコンポーネント）が見つかりません");
            return;
        }
        
        // ステータスを読み込む
        List<int> stats = data.GetEnemyData(enemyId);
        if (stats != null && stats.Count >= 3)
        {
            MAXHP = stats[0];
            HP = MAXHP;  // 初期HPは最大HPと同じ
            ATK = stats[1];
            DEF = stats[2];
            
            // BUFFデータがある場合は読み込む（現在は0だが将来の拡張用）
            if (stats.Count > 7)
            {
                BUFF[0] = stats[3];  // 持続ダメージ
                BUFF[1] = stats[4];  // ダメージダウン
                BUFF[2] = stats[5];  // ダメージアップ
                BUFF[3] = stats[6];  // 防御力ダウン
                BUFF[4] = stats[7];  // 防御力アップ
            }
            
            Debug.Log($"enemy_L: 敵ID {enemyId} のステータスを読み込みました (HP:{HP}/{MAXHP}, ATK:{ATK}, DEF:{DEF})");
        }
        else
        {
            Debug.LogWarning($"enemy_L: 敵ID {enemyId} のデータが見つかりません");
        }
    }
    
    /// <summary>
    /// Awake時に自動的にステータスを読み込む（enemyIdが設定されている場合）
    /// </summary>
    private void Awake()
    {
        if (enemyId >= 0)
        {
            LoadStatsFromDataFile();
        }
    }

    /// <summary>
    /// 敵IDから敵データを取得
    /// </summary>
    /// <param name="enemyId">敵ID (0-17)</param>
    /// <returns>敵データのリスト [MAXHP, ATK, DEF, BUFF...]</returns>
    public List<int> GetEnemyData(int enemyId)
    {
        if (enemy_list.TryGetValue(enemyId, out List<int> data))
        {
            return data;
        }
        Debug.LogWarning($"enemy_L: 敵ID {enemyId} が見つかりません");
        return new List<int>() { 100, 15, 10, 0, 0, 0, 0, 0 }; // デフォルト値
    }

    /// <summary>
    /// 敵IDから敵名を取得
    /// </summary>
    /// <param name="enemyId">敵ID (0-17)</param>
    /// <returns>敵名（日本語）</returns>
    public string GetEnemyName(int enemyId)
    {
        if (enemy_name_list.TryGetValue(enemyId, out string name))
        {
            return name;
        }
        Debug.LogWarning($"enemy_L: 敵ID {enemyId} の名前が見つかりません");
        return $"敵{enemyId}";
    }

    /// <summary>
    /// 敵IDから画像ファイル名を取得
    /// </summary>
    /// <param name="enemyId">敵ID (0-17)</param>
    /// <returns>画像ファイル名（拡張子なし）</returns>
    public string GetEnemyImageName(int enemyId)
    {
        if (enemy_image_map.TryGetValue(enemyId, out string imageName))
        {
            return imageName;
        }
        Debug.LogWarning($"enemy_L: 敵ID {enemyId} の画像ファイル名が見つかりません");
        return "";
    }

    /// <summary>
    /// 敵IDから敵タイプを取得
    /// </summary>
    /// <param name="enemyId">敵ID (0-17)</param>
    /// <returns>敵タイプ</returns>
    public EnemyType GetEnemyType(int enemyId)
    {
        if (enemy_type_map.TryGetValue(enemyId, out EnemyType type))
        {
            return type;
        }
        Debug.LogWarning($"enemy_L: 敵ID {enemyId} のタイプが見つかりません");
        return EnemyType.Common;
    }

    /// <summary>
    /// 敵IDから出現惑星を取得
    /// </summary>
    /// <param name="enemyId">敵ID (0-17)</param>
    /// <returns>出現惑星名（"All"は全惑星、"None"は最終ボス用）</returns>
    public string GetEnemyPlanet(int enemyId)
    {
        if (enemy_planet_map.TryGetValue(enemyId, out string planet))
        {
            return planet;
        }
        Debug.LogWarning($"enemy_L: 敵ID {enemyId} の出現惑星が見つかりません");
        return "All";
    }

    /// <summary>
    /// 指定した惑星に出現する敵IDのリストを取得
    /// </summary>
    /// <param name="planetName">惑星名（BattleScreen.PlanetTypeの文字列、または"All"）</param>
    /// <returns>敵IDのリスト</returns>
    public List<int> GetEnemyIdsByPlanet(string planetName)
    {
        List<int> result = new List<int>();
        foreach (var kvp in enemy_planet_map)
        {
            if (kvp.Value == planetName || kvp.Value == "All")
            {
                result.Add(kvp.Key);
            }
        }
        return result;
    }

    /// <summary>
    /// 指定した惑星とタイプに一致する敵IDのリストを取得
    /// </summary>
    /// <param name="planetName">惑星名（BattleScreen.PlanetTypeの文字列、または"All"）</param>
    /// <param name="type">敵タイプ</param>
    /// <returns>敵IDのリスト</returns>
    public List<int> GetEnemyIdsByPlanetAndType(string planetName, EnemyType type)
    {
        List<int> result = new List<int>();
        foreach (var kvp in enemy_planet_map)
        {
            int enemyId = kvp.Key;
            if ((kvp.Value == planetName || kvp.Value == "All") && 
                GetEnemyType(enemyId) == type)
            {
                result.Add(enemyId);
            }
        }
        return result;
    }

    /// <summary>
    /// 指定したタイプの敵IDのリストを取得
    /// </summary>
    /// <param name="type">敵タイプ</param>
    /// <returns>敵IDのリスト</returns>
    public List<int> GetEnemyIdsByType(EnemyType type)
    {
        List<int> result = new List<int>();
        foreach (var kvp in enemy_type_map)
        {
            if (kvp.Value == type)
            {
                result.Add(kvp.Key);
            }
        }
        return result;
    }
}
