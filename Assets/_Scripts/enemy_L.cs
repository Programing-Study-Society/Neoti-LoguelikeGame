using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵のタイプ
/// </summary>
public enum EnemyType
{
    Common,      // 雑魚（全惑星に出現）
    Unique,      // 固有敵（惑星固有の中盤の敵）
    MidBoss,     // 中ボス（惑星固有、中盤の敵2体のうちどちらかから選択）
    Boss,        // ステージボス（惑星固有、Act終了）
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
    /// Key: 敵ID (0-22, 全23体)
    /// Value: [MAXHP, ATK, DEF, 持続ダメージ, ダメージダウン, ダメージアップ, 防御力ダウン, 防御力アップ]
    /// </summary>
    public Dictionary<int, List<int>> enemy_list = new Dictionary<int, List<int>>()
    {
        // =========================
        // 全惑星共通（雑魚：序盤の敵）
        // HP: 500-600, DEF: 100, ATK: 200
        // =========================
        {0, new List<int>() {600, 200, 100, 0, 0, 0, 0, 0}},   // Claw_Loader (雑魚)
        {1, new List<int>() {500, 200, 100, 0, 0, 0, 0, 0}},   // Omni-Roid (雑魚)
        
        // =========================
        // Volcano_Planet (固有敵×2 + ステージボス×1)
        // 固有敵: HP: 1000-1500, DEF: 150, ATK: 250
        // ステージボス: HP: 3000, DEF: 300, ATK: 500
        // =========================
        {2, new List<int>() {1200, 250, 150, 0, 0, 0, 0, 0}},   // Welder (固有敵)
        {3, new List<int>() {1500, 250, 150, 0, 0, 0, 0, 0}},   // Fire_Ant (固有敵)
        {4, new List<int>() {3000, 500, 300, 0, 0, 0, 0, 0}},  // Typhon (ステージボス)
        
        // =========================
        // Ice_Planet (固有敵×2 + ステージボス×1)
        // =========================
        {5, new List<int>() {1000, 250, 150, 0, 0, 0, 0, 0}},  // Prospector (固有敵)
        {6, new List<int>() {1300, 250, 150, 0, 0, 0, 0, 0}},   // Cable_Rigger (固有敵)
        {7, new List<int>() {3000, 500, 300, 0, 0, 0, 0, 0}},  // Boreas (ステージボス)
        
        // =========================
        // Forest_Planet (固有敵×2 + ステージボス×1)
        // =========================
        {8, new List<int>() {1100, 250, 150, 0, 0, 0, 0, 0}},   // Strider (固有敵)
        {9, new List<int>() {1400, 250, 150, 0, 0, 0, 0, 0}},   // Timber_Jack (固有敵)
        {10, new List<int>() {3000, 500, 300, 0, 0, 0, 0, 0}},  // Briareus (ステージボス)
        
        // =========================
        // Old_Empire_Planet (固有敵×2 + ステージボス×1)
        // =========================
        {11, new List<int>() {1000, 250, 150, 0, 0, 0, 0, 0}},  // Enforcer (固有敵)
        {12, new List<int>() {1500, 250, 150, 0, 0, 0, 0, 0}}, // Juggernaut (固有敵)
        {13, new List<int>() {3000, 500, 300, 0, 0, 0, 0, 0}}, // Talos_Zero (ステージボス)
        
        // =========================
        // Desert_Planet (固有敵×2 + ステージボス×1)
        // =========================
        {14, new List<int>() {1200, 250, 150, 0, 0, 0, 0, 0}},  // Vulture (固有敵)
        {15, new List<int>() {1100, 250, 150, 0, 0, 0, 0, 0}},  // Louver_Ray (固有敵)
        {16, new List<int>() {3000, 500, 300, 0, 0, 0, 0, 0}}, // Seth_Gigas (ステージボス)
        
        // =========================
        // 最終ボス
        // HP: 100000, DEF: 10000, ATK: 12000
        // =========================
        {17, new List<int>() {100000, 12000, 10000, 0, 0, 0, 0, 0}},  // 最終ボス
        
        // =========================
        // 中ボス（各惑星1体、名前・画像は固有敵2体のうちどちらかから選択）
        // HP: 2000, DEF: 200, ATK: 300
        // =========================
        {18, new List<int>() {2000, 300, 200, 0, 0, 0, 0, 0}}, // 中ボス（Volcano_Planet: WelderまたはFire_Antから選択）
        {19, new List<int>() {2000, 300, 200, 0, 0, 0, 0, 0}}, // 中ボス（Ice_Planet: ProspectorまたはCable_Riggerから選択）
        {20, new List<int>() {2000, 300, 200, 0, 0, 0, 0, 0}}, // 中ボス（Forest_Planet: StriderまたはTimber_Jackから選択）
        {21, new List<int>() {2000, 300, 200, 0, 0, 0, 0, 0}}, // 中ボス（Old_Empire_Planet: EnforcerまたはJuggernautから選択）
        {22, new List<int>() {2000, 300, 200, 0, 0, 0, 0, 0}}  // 中ボス（Desert_Planet: VultureまたはLouver_Rayから選択）
    };

    /// <summary>
    /// 敵IDと名前の対応表
    /// 中ボス（ID 18-22）の名前は固有敵2体のうちどちらかから動的に選択される
    /// </summary>
    public Dictionary<int, string> enemy_name_list = new Dictionary<int, string>()
    {
        // =========================
        // 全惑星共通（雑魚）
        // =========================
        {0, "クローローダー"},
        {1, "オムニロイド"},
        
        // =========================
        // Volcano_Planet
        // =========================
        {2, "ウェルダー"},
        {3, "ファイアーアント"},
        {4, "タイフォン"},
        
        // =========================
        // Ice_Planet
        // =========================
        {5, "プロスペクター"},
        {6, "ケーブルリガー"},
        {7, "ボレアス"},
        
        // =========================
        // Forest_Planet
        // =========================
        {8, "ストライダー"},
        {9, "ティンバージャック"},
        {10, "ブリアレウス"},
        
        // =========================
        // Old_Empire_Planet
        // =========================
        {11, "エンフォーサー"},
        {12, "ジャガーノート"},
        {13, "タロスゼロ"},
        
        // =========================
        // Desert_Planet
        // =========================
        {14, "バルチャー"},
        {15, "ルーバーレイ"},
        {16, "セト・ギガス"},
        
        // =========================
        // 最終ボス
        // =========================
        {17, "バリアント"},
        
        // =========================
        // 中ボス（名前は動的に選択される）
        // =========================
        {18, "中ボス（Volcano）"}, // WelderまたはFire_Antから選択
        {19, "中ボス（Ice）"},     // ProspectorまたはCable_Riggerから選択
        {20, "中ボス（Forest）"},   // StriderまたはTimber_Jackから選択
        {21, "中ボス（Old_Empire）"}, // EnforcerまたはJuggernautから選択
        {22, "中ボス（Desert）"}   // VultureまたはLouver_Rayから選択
    };

    /// <summary>
    /// 敵IDと画像ファイル名の対応表
    /// Resources/enemies/ フォルダから読み込む際に使用
    /// 中ボス（ID 18-22）の画像は固有敵2体のうちどちらかから動的に選択される
    /// </summary>
    public Dictionary<int, string> enemy_image_map = new Dictionary<int, string>()
    {
        // =========================
        // 全惑星共通（雑魚）
        // =========================
        {0, "Claw_Loader"},
        {1, "Omni-Roid"},
        
        // =========================
        // Volcano_Planet
        // =========================
        {2, "Welder"},
        {3, "Fire_Ant"},
        {4, "Typhon"},
        
        // =========================
        // Ice_Planet
        // =========================
        {5, "Prospector"},
        {6, "Cable_Rigger"},
        {7, "Boreas"},
        
        // =========================
        // Forest_Planet
        // =========================
        {8, "Strider"},
        {9, "Timber_Jack"},
        {10, "Briareus"},
        
        // =========================
        // Old_Empire_Planet
        // =========================
        {11, "Enforcer"},
        {12, "Juggernaut"},
        {13, "Talos_Zero"},
        
        // =========================
        // Desert_Planet
        // =========================
        {14, "Vulture"},
        {15, "Louver_Ray"},
        {16, "Seth_Gigas"},
        
        // =========================
        // 最終ボス
        // =========================
        {17, "Variant"},
        
        // =========================
        // 中ボス（画像は動的に選択される）
        // =========================
        {18, ""},             // 中ボス（WelderまたはFire_Antから選択）
        {19, ""},             // 中ボス（ProspectorまたはCable_Riggerから選択）
        {20, ""},             // 中ボス（StriderまたはTimber_Jackから選択）
        {21, ""},             // 中ボス（EnforcerまたはJuggernautから選択）
        {22, ""}              // 中ボス（VultureまたはLouver_Rayから選択）
    };

    /// <summary>
    /// 敵IDと敵タイプの対応表
    /// </summary>
    public Dictionary<int, EnemyType> enemy_type_map = new Dictionary<int, EnemyType>()
    {
        // =========================
        // 全惑星共通（雑魚）
        // =========================
        {0, EnemyType.Common},    // Claw_Loader
        {1, EnemyType.Common},    // Omni-Roid
        
        // =========================
        // Volcano_Planet
        // =========================
        {2, EnemyType.Unique},    // Welder (固有敵)
        {3, EnemyType.Unique},    // Fire_Ant (固有敵)
        {4, EnemyType.Boss},      // Typhon (ステージボス)
        
        // =========================
        // Ice_Planet
        // =========================
        {5, EnemyType.Unique},    // Prospector (固有敵)
        {6, EnemyType.Unique},    // Cable_Rigger (固有敵)
        {7, EnemyType.Boss},      // Boreas (ステージボス)
        
        // =========================
        // Forest_Planet
        // =========================
        {8, EnemyType.Unique},    // Strider (固有敵)
        {9, EnemyType.Unique},    // Timber_Jack (固有敵)
        {10, EnemyType.Boss},     // Briareus (ステージボス)
        
        // =========================
        // Old_Empire_Planet
        // =========================
        {11, EnemyType.Unique},   // Enforcer (固有敵)
        {12, EnemyType.Unique},   // Juggernaut (固有敵)
        {13, EnemyType.Boss},     // Talos_Zero (ステージボス)
        
        // =========================
        // Desert_Planet
        // =========================
        {14, EnemyType.Unique},   // Vulture (固有敵)
        {15, EnemyType.Unique},   // Louver_Ray (固有敵)
        {16, EnemyType.Boss},     // Seth_Gigas (ステージボス)
        
        // =========================
        // 最終ボス
        // =========================
        {17, EnemyType.FinalBoss},
        
        // =========================
        // 中ボス（各惑星1体）
        // =========================
        {18, EnemyType.MidBoss},   // 中ボス（Volcano_Planet）
        {19, EnemyType.MidBoss},   // 中ボス（Ice_Planet）
        {20, EnemyType.MidBoss},   // 中ボス（Forest_Planet）
        {21, EnemyType.MidBoss},   // 中ボス（Old_Empire_Planet）
        {22, EnemyType.MidBoss}    // 中ボス（Desert_Planet）
    };

    /// <summary>
    /// 敵IDと出現惑星の対応表
    /// "All"は全惑星に出現（雑魚用）
    /// BattleScreen.PlanetTypeの文字列と一致させる
    /// </summary>
    public Dictionary<int, string> enemy_planet_map = new Dictionary<int, string>()
    {
        // =========================
        // 全惑星共通（雑魚）
        // =========================
        {0, "All"},               // Claw_Loader
        {1, "All"},               // Omni-Roid
        
        // =========================
        // Volcano_Planet
        // =========================
        {2, "Volcano_Planet"},     // Welder (固有敵)
        {3, "Volcano_Planet"},     // Fire_Ant (固有敵)
        {4, "Volcano_Planet"},     // Typhon (ステージボス)
        
        // =========================
        // Ice_Planet
        // =========================
        {5, "Ice_Planet"},         // Prospector (固有敵)
        {6, "Ice_Planet"},         // Cable_Rigger (固有敵)
        {7, "Ice_Planet"},         // Boreas (ステージボス)
        
        // =========================
        // Forest_Planet
        // =========================
        {8, "Forest_Planet"},      // Strider (固有敵)
        {9, "Forest_Planet"},      // Timber_Jack (固有敵)
        {10, "Forest_Planet"},     // Briareus (ステージボス)
        
        // =========================
        // Old_Empire_Planet
        // =========================
        {11, "Old_Empire_Planet"}, // Enforcer (固有敵)
        {12, "Old_Empire_Planet"}, // Juggernaut (固有敵)
        {13, "Old_Empire_Planet"}, // Talos_Zero (ステージボス)
        
        // =========================
        // Desert_Planet
        // =========================
        {14, "Desert_Planet"},     // Vulture (固有敵)
        {15, "Desert_Planet"},     // Louver_Ray (固有敵)
        {16, "Desert_Planet"},     // Seth_Gigas (ステージボス)
        
        // =========================
        // 最終ボス
        // =========================
        {17, "None"},              // 最終ボス
        
        // =========================
        // 中ボス（各惑星1体）
        // =========================
        {18, "Volcano_Planet"},    // 中ボス（Volcano_Planet）
        {19, "Ice_Planet"},        // 中ボス（Ice_Planet）
        {20, "Forest_Planet"},     // 中ボス（Forest_Planet）
        {21, "Old_Empire_Planet"}, // 中ボス（Old_Empire_Planet）
        {22, "Desert_Planet"}      // 中ボス（Desert_Planet）
    };
    
    /// <summary>
    /// 各惑星の中盤の敵（固有敵）2体のIDを保持
    /// 中ボスの名前・画像はこの2体のうちどちらかから選択される
    /// </summary>
    private Dictionary<string, List<int>> planetUniqueEnemyIds = new Dictionary<string, List<int>>()
    {
        {"Volcano_Planet", new List<int>() {2, 3}},  // Welder, Fire_Ant
        {"Ice_Planet", new List<int>() {5, 6}},      // Prospector, Cable_Rigger
        {"Forest_Planet", new List<int>() {8, 9}},   // Strider, Timber_Jack
        {"Old_Empire_Planet", new List<int>() {11, 12}}, // Enforcer, Juggernaut
        {"Desert_Planet", new List<int>() {14, 15}}  // Vulture, Louver_Ray
    };
    
    /// <summary>
    /// 各惑星の中ボスID
    /// </summary>
    private Dictionary<string, int> planetMidBossIds = new Dictionary<string, int>()
    {
        {"Volcano_Planet", 18},
        {"Ice_Planet", 19},
        {"Forest_Planet", 20},
        {"Old_Empire_Planet", 21},
        {"Desert_Planet", 22}
    };
    
    /// <summary>
    /// 中ボスの名前・画像の元となる固有敵IDを保持（動的に設定される）
    /// Key: 中ボスID (18-22), Value: 選択された固有敵ID (2,3,5,6,8,9,11,12,14,15のいずれか)
    /// </summary>
    private Dictionary<int, int> midBossSourceEnemyId = new Dictionary<int, int>();

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
    /// <param name="enemyId">敵ID (0-22)</param>
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
    /// 中ボス（ID 18-22）の場合は、選択された固有敵の名前を返す
    /// </summary>
    /// <param name="enemyId">敵ID (0-22)</param>
    /// <returns>敵名（日本語）</returns>
    public string GetEnemyName(int enemyId)
    {
        // 中ボスの場合は、選択された固有敵の名前を返す
        if (midBossSourceEnemyId.TryGetValue(enemyId, out int sourceEnemyId))
        {
            // 再帰を避けるため、直接enemy_name_listから取得
            if (enemy_name_list.TryGetValue(sourceEnemyId, out string sourceName))
            {
                return sourceName;
            }
        }
        
        if (enemy_name_list.TryGetValue(enemyId, out string name))
        {
            return name;
        }
        Debug.LogWarning($"enemy_L: 敵ID {enemyId} の名前が見つかりません");
        return $"敵{enemyId}";
    }

    /// <summary>
    /// 敵IDから画像ファイル名を取得
    /// 中ボス（ID 18-22）の場合は、選択された固有敵の画像ファイル名を返す
    /// </summary>
    /// <param name="enemyId">敵ID (0-22)</param>
    /// <returns>画像ファイル名（拡張子なし）</returns>
    public string GetEnemyImageName(int enemyId)
    {
        // 中ボスの場合は、選択された固有敵の画像ファイル名を返す
        if (midBossSourceEnemyId.TryGetValue(enemyId, out int sourceEnemyId))
        {
            // 再帰を避けるため、直接enemy_image_mapから取得
            if (enemy_image_map.TryGetValue(sourceEnemyId, out string sourceImageName))
            {
                return sourceImageName;
            }
        }
        
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
    /// <param name="enemyId">敵ID (0-22)</param>
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
    /// <param name="enemyId">敵ID (0-22)</param>
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
    /// 中ボスの名前・画像を初期化（惑星名から中盤の敵2体のうちどちらかを選択）
    /// 中ボスステージが開始される前に呼び出す必要がある
    /// </summary>
    /// <param name="planetName">惑星名</param>
    public void InitializeMidBoss(string planetName)
    {
        if (!planetUniqueEnemyIds.TryGetValue(planetName, out List<int> uniqueEnemyIds))
        {
            Debug.LogWarning($"enemy_L: 惑星 '{planetName}' の固有敵が見つかりません");
            return;
        }
        
        if (!planetMidBossIds.TryGetValue(planetName, out int midBossId))
        {
            Debug.LogWarning($"enemy_L: 惑星 '{planetName}' の中ボスIDが見つかりません");
            return;
        }
        
        // 中盤の敵2体のうちどちらかをランダムに選択
        int selectedUniqueEnemyId = uniqueEnemyIds[Random.Range(0, uniqueEnemyIds.Count)];
        
        // 中ボスの名前・画像の元となる固有敵IDを保存
        midBossSourceEnemyId[midBossId] = selectedUniqueEnemyId;
        
        Debug.Log($"enemy_L: 中ボス（ID:{midBossId}）を初期化 - 惑星:{planetName}, 選択された固有敵ID:{selectedUniqueEnemyId}, 名前:{GetEnemyName(selectedUniqueEnemyId)}");
    }
    
    /// <summary>
    /// 指定した惑星の中ボスIDを取得
    /// </summary>
    /// <param name="planetName">惑星名</param>
    /// <returns>中ボスID（見つからない場合は-1）</returns>
    public int GetMidBossId(string planetName)
    {
        if (planetMidBossIds.TryGetValue(planetName, out int midBossId))
        {
            return midBossId;
        }
        Debug.LogWarning($"enemy_L: 惑星 '{planetName}' の中ボスIDが見つかりません");
        return -1;
    }
    
    /// <summary>
    /// 指定した惑星の固有敵（中盤の敵）IDリストを取得
    /// </summary>
    /// <param name="planetName">惑星名</param>
    /// <returns>固有敵IDのリスト（2体）</returns>
    public List<int> GetUniqueEnemyIds(string planetName)
    {
        if (planetUniqueEnemyIds.TryGetValue(planetName, out List<int> uniqueEnemyIds))
        {
            return new List<int>(uniqueEnemyIds); // コピーを返す
        }
        Debug.LogWarning($"enemy_L: 惑星 '{planetName}' の固有敵が見つかりません");
        return new List<int>();
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
