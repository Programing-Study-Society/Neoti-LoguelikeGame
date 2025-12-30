# バトルシステム Hierarchy階層図

## 推奨されるHierarchy構造

```
Canvas (Canvas)
├─ BattleScreen (GameObject + BattleScreen.cs)
│  ├─ BackgroundImage (Image) - 背景画像
│  ├─ PlayerImage (Image) - プレイヤー画像
│  ├─ PlayerHPBar (Slider) - プレイヤーHPバー
│  ├─ PlayerHPText (TextMeshProUGUI) - プレイヤーHPテキスト
│  │
│  ├─ EnemyContainer (GameObject + HorizontalLayoutGroup)
│  │  ├─ EnemySlot_0 (GameObject + EnemySlot.cs) - 敵1
│  │  ├─ EnemySlot_1 (GameObject + EnemySlot.cs) - 敵2
│  │  └─ ... (動的に生成される)
│  │
│  ├─ ItemSelectionPanel (GameObject)
│  │  ├─ ScrollRect (ScrollRect)
│  │  │  └─ Content (RectTransform)
│  │  │     └─ ItemGridContainer (GameObject + GridLayoutGroup)
│  │  │        ├─ BattleItemSlot_0 (GameObject + BattleItemSlot.cs) - 動的に生成
│  │  │        ├─ BattleItemSlot_1 (GameObject + BattleItemSlot.cs) - 動的に生成
│  │  │        └─ ... (動的に生成される)
│  │  └─ ItemSelectionTitleText (TextMeshProUGUI) - オプション
│  │
│  ├─ AttackButtonPanel (GameObject)
│  │  └─ AttackButtonContainer (GameObject + HorizontalLayoutGroup)
│  │     ├─ AttackButton_0 (GameObject + Button) - 動的に生成
│  │     ├─ AttackButton_1 (GameObject + Button) - 動的に生成
│  │     └─ ... (動的に生成される)
│  │
│  └─ DamageTextContainer (GameObject) - オプション（推奨）
│     └─ (ダメージテキストは動的に生成される)
│
├─ battle_system (GameObject + battle_system.cs)
│  └─ (ロジック用、非表示のGameObject)
│
├─ summary (GameObject + summary.cs)
│  └─ (GameManager)
│
├─ item (GameObject + item.cs)
│  └─ (アイテムデータ管理)
│
├─ stage_data (GameObject + stage_data.cs)
│  └─ (ステージ進行度データ)
│
├─ EnemySpawnManager (GameObject + EnemySpawnManager.cs)
│  └─ (敵出現管理)
│
└─ player (GameObject + player.cs)
   └─ (プレイヤーデータ)
```

## 各オブジェクトの説明

### Canvas
- **役割**: UI全体の親オブジェクト
- **コンポーネント**: Canvas, GraphicRaycaster

### BattleScreen
- **役割**: バトル画面のUI管理
- **コンポーネント**: BattleScreen.cs
- **子オブジェクト**:
  - **BackgroundImage**: 背景画像表示
  - **PlayerImage**: プレイヤー画像表示
  - **PlayerHPBar**: プレイヤーHPバー（Slider）
  - **PlayerHPText**: プレイヤーHP数値表示
  - **EnemyContainer**: 敵を配置する親（HorizontalLayoutGroup）
  - **ItemSelectionPanel**: アイテム選択パネル
  - **AttackButtonPanel**: 攻撃ボタンパネル
  - **DamageTextContainer**: ダメージテキストの親（オプション、推奨）

### battle_system
- **役割**: バトルロジック管理
- **コンポーネント**: battle_system.cs
- **注意**: ロジック用なので、GameObjectは非表示（`SetActive(false)`）でもOK

### summary
- **役割**: ゲーム全体の管理（GameManager）
- **コンポーネント**: summary.cs

### item
- **役割**: アイテムデータ管理
- **コンポーネント**: item.cs

### stage_data
- **役割**: ステージ進行度データ管理
- **コンポーネント**: stage_data.cs

### EnemySpawnManager
- **役割**: 敵出現管理
- **コンポーネント**: EnemySpawnManager.cs

### player
- **役割**: プレイヤーデータ管理
- **コンポーネント**: player.cs

## 動的に生成されるオブジェクト

### 敵スロット（EnemySlot）
- **生成タイミング**: バトル開始時
- **生成場所**: `EnemyContainer`の子
- **プレハブ**: `EnemySlot.prefab`

### アイテムスロット（BattleItemSlot）
- **生成タイミング**: アイテム選択パネル表示時
- **生成場所**: `ItemGridContainer`の子
- **プレハブ**: `BattleItemSlot.prefab`

### 攻撃ボタン
- **生成タイミング**: アイテム選択後
- **生成場所**: `AttackButtonContainer`の子
- **プレハブ**: `AttackButtonPrefab`

### ダメージテキスト
- **生成タイミング**: ダメージ発生時
- **生成場所**: `DamageTextContainer`の子（またはBattleScreen直下）
- **プレハブ**: `DamageText.prefab`

## 重要なポイント

1. **BattleScreenはCanvasの子オブジェクト**として配置
2. **battle_systemは独立したGameObject**（Canvasの子でも親でもOK）
3. **動的生成されるオブジェクト**は適切な親オブジェクトの下に生成される
4. **DamageTextContainer**はBattleScreen直下に作成することを推奨（管理しやすい）

## Inspector設定の確認ポイント

### BattleScreen.cs
- ✅ BackgroundImage
- ✅ PlayerImage, PlayerHPBar, PlayerHPText
- ✅ EnemyContainer
- ✅ ItemSelectionPanel, ItemGridContainer, BattleItemSlotPrefab
- ✅ AttackButtonPanel, AttackButtonContainer, AttackButtonPrefab
- ✅ DamageTextPrefab, DamageTextParent（DamageTextContainer推奨）
- ✅ enemyData, itemData, battleSystem

### battle_system.cs
- ✅ EnemyPrefab
- ✅ EnemySpawnManager, EnemyDataFile, StageData, ItemMaster
- ✅ BattleScreen
- ✅ Player

