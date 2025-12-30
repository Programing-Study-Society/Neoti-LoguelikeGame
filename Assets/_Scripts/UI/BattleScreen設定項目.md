# BattleScreen インスペクター設定項目

## 必須設定項目

### 1. 背景設定
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Background Image** | `Image` | 背景画像を表示するImageコンポーネント | Hierarchyの`Background`オブジェクトをドラッグ&ドロップ |

### 2. 惑星背景画像（6個）
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Forest Planet Sprite** | `Sprite` | Forest_Planet用の背景画像 | `Sprites/UI/Forest_Planet.png`を設定 |
| **Ice Planet Sprite** | `Sprite` | Ice_Planet用の背景画像 | `Sprites/UI/Ice_Planet.png`を設定 |
| **Old Empire Planet Sprite** | `Sprite` | Old_Empire_Planet用の背景画像 | `Sprites/UI/Old_Empire_Planet.png`を設定 |
| **Desert Planet Sprite** | `Sprite` | Desert_Planet用の背景画像（未実装） | オプション |
| **Volcano Planet Sprite** | `Sprite` | Volcano_Planet用の背景画像（未実装） | オプション |
| **Default Background Sprite** | `Sprite` | デフォルト背景画像（惑星が不明な場合） | オプション |

### 3. プレイヤー表示（3個）
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Player Image** | `Image` | プレイヤー画像を表示するImageコンポーネント | Hierarchyの`PlayerImage`オブジェクトをドラッグ&ドロップ |
| **Player HP Bar** | `Slider` | プレイヤーHPバー（Sliderコンポーネント） | Hierarchyの`PlayerHPBar`オブジェクトをドラッグ&ドロップ |
| **Player HP Text** | `TextMeshProUGUI` | プレイヤーHPテキスト（例: "100/200"） | Hierarchyの`PlayerHPText`オブジェクトをドラッグ&ドロップ |

### 4. 敵表示（2個）
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Enemy Container** | `Transform` | 敵を配置する親オブジェクト（HorizontalLayoutGroup付き推奨） | Hierarchyの`EnemyContainer`オブジェクトをドラッグ&ドロップ |
| **Enemy Slot Prefab** | `GameObject` | 敵表示用のプレハブ（EnemySlot.cs付き） | `Prefabs/UI/EnemySlot.prefab`をドラッグ&ドロップ |

### 5. アイテム選択パネル（4個）
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Item Selection Panel** | `GameObject` | アイテム選択パネル（中央表示） | Hierarchyの`ItemSelectionPanel`オブジェクトをドラッグ&ドロップ |
| **Item Grid Container** | `Transform` | アイテムグリッドの親（GridLayoutGroup付き） | Hierarchyの`ItemGridContainer`オブジェクトをドラッグ&ドロップ |
| **Battle Item Slot Prefab** | `GameObject` | バトル用アイテムスロットプレハブ（BattleItemSlot.cs付き） | `Prefabs/UI/BattleItemSlot.prefab`をドラッグ&ドロップ |
| **Item Selection Title Text** | `TextMeshProUGUI` | タイトルテキスト（オプション、例: "アイテムを選択"） | Hierarchyの`ItemSelectionTitleText`オブジェクトをドラッグ&ドロップ（オプション） |

### 6. 攻撃ボタン（3個）
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Attack Button Panel** | `GameObject` | 攻撃ボタンパネル（アイテム選択後に表示） | Hierarchyの`AttackButtonPanel`オブジェクトをドラッグ&ドロップ |
| **Attack Button Container** | `Transform` | 攻撃ボタンの親（HorizontalLayoutGroupなど） | Hierarchyの`AttackButtonContainer`オブジェクトをドラッグ&ドロップ |
| **Attack Button Prefab** | `GameObject` | 攻撃ボタンプレハブ | `Prefabs/UI/AttackButton.prefab`をドラッグ&ドロップ |

### 7. データ参照（3個）
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Enemy Data** | `enemy_L` | enemy_L.csへの参照（敵画像ファイル名取得用） | シーン内の`enemy_L`コンポーネントがアタッチされたオブジェクトをドラッグ&ドロップ |
| **Item Data** | `item` | item.csへの参照（アイテムデータ取得用） | シーン内の`item`コンポーネントがアタッチされたオブジェクトをドラッグ&ドロップ |
| **Battle System** | `battle_system` | バトルシステム（ロジック側、A案：ロジックが正） | シーン内の`battle_system`コンポーネントがアタッチされたオブジェクトをドラッグ&ドロップ |

## オプション設定項目

### 8. プレイヤー表示設定
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **Player Placeholder Color** | `Color` | プレイヤー画像のプレースホルダー色 | `Color.blue` |

### 9. 敵表示設定
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **Enemy Placeholder Color** | `Color` | 敵画像のプレースホルダー色 | `Color.red` |

### 10. HPバー設定
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **HP Bar Color** | `Color` | HPバーの色 | `Color.red` |
| **HP Bar Background Color** | `Color` | HPバーの背景色 | `Color.black` |

### 11. テスト用設定
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Test Battle Manager** | `TestBattleManager` | テスト用（テスト時のみ使用） | シーン内の`TestBattleManager`コンポーネントをドラッグ&ドロップ（オプション） |

## イベント設定

### 12. バトル確認イベント
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **On Battle Confirmed** | `UnityEvent` | 戦闘終了確認時のイベント | Inspectorのイベント欄で、呼び出したいメソッドを設定 |

## プレハブ一覧

### 必要なプレハブ（3個）

1. **EnemySlot.prefab**
   - 場所: `Prefabs/UI/EnemySlot.prefab`
   - コンポーネント: `EnemySlot.cs`がアタッチされている必要がある
   - 構造: Image（敵画像）+ Slider（HPバー）+ TextMeshProUGUI（HPテキスト）

2. **BattleItemSlot.prefab**
   - 場所: `Prefabs/UI/BattleItemSlot.prefab`
   - コンポーネント: `BattleItemSlot.cs`がアタッチされている必要がある
   - 構造: Image（アイテム画像）+ TextMeshProUGUI（アイテム名）+ TextMeshProUGUI（所持数）+ Button

3. **AttackButton.prefab**
   - 場所: `Prefabs/UI/AttackButton.prefab`
   - 構造: Button + TextMeshProUGUI（ボタンテキスト）

## Hierarchy構造の例

```
BattleScreen (GameObject, BattleScreen.cs)
├── Background (Image)
├── PlayerImage (Image)
├── PlayerHPBar (Slider)
├── PlayerHPText (TextMeshProUGUI)
├── EnemyContainer (GameObject, HorizontalLayoutGroup)
│   └── [動的生成] EnemySlot × n
├── ItemSelectionPanel (GameObject)
│   ├── ItemSelectionTitleText (TextMeshProUGUI) [オプション]
│   └── ItemGridContainer (GameObject, GridLayoutGroup)
│       └── [動的生成] BattleItemSlot × n
└── AttackButtonPanel (GameObject)
    └── AttackButtonContainer (GameObject, HorizontalLayoutGroup)
        └── [動的生成] AttackButton × n
```

## 設定手順

1. **Hierarchy構造の確認**
   - `BattleScreen`オブジェクトが存在する
   - 上記のHierarchy構造に従って子オブジェクトが作成されている

2. **BattleScreenコンポーネントの設定**
   - `BattleScreen`オブジェクトを選択
   - Inspectorで`BattleScreen`コンポーネントを確認
   - 上記の各フィールドに適切なオブジェクト/スプライト/プレハブを設定

3. **プレハブの確認**
   - `EnemySlot.prefab`が`Prefabs/UI/`に存在する
   - `BattleItemSlot.prefab`が`Prefabs/UI/`に存在する
   - `AttackButton.prefab`が`Prefabs/UI/`に存在する

4. **Resourcesフォルダの確認**
   - `Resources/enemies/`フォルダに敵画像が配置されている
   - `Resources/itemphoto/`フォルダにアイテム画像が配置されている

## 注意事項

- **Enemy Container**には`HorizontalLayoutGroup`コンポーネントが必要です
- **Item Grid Container**には`GridLayoutGroup`コンポーネントが必要です
- **Attack Button Container**には`HorizontalLayoutGroup`コンポーネントが必要です
- **Battle System**が未設定の場合、一部の機能が動作しません（A案：ロジックが正）
- **Enemy Data**が未設定の場合、敵名が「敵{ID}」と表示されます
- **Item Data**が未設定の場合、アイテム名が「アイテム{ID}」と表示されます

