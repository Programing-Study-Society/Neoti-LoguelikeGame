# Unity UIセットアップ手順

最終更新: 2025/12/11

---

## 開発環境

| 項目 | バージョン |
|------|-----------|
| Unity | 2021.3.45 LTS |

### TextMeshProについて
- Unity 2021以降では **TextMeshPro がデフォルト**です
- 「UI → Button - TextMeshPro」「UI → Text - TextMeshPro」を使用してください
- 初回使用時に「TMP Importer」が表示されたら **Import TMP Essentials** をクリック

---

## 現在のUI構成

| 画面 | 状態 | スクリプト |
|------|------|-----------|
| タイトル画面 | 別担当 | - |
| マップ画面 | 未着手（仕様未確定） | - |
| 宝箱画面 | ✅ 完成 | TreasureScreen.cs |
| バトル画面 | 作成中 | BattleScreen.cs |

---

## オブジェクト階層構造

```
Hierarchy
├── Canvas (UI全体の親)
│   ├── EventSystem (自動生成)
│   │
│   ├── TreasureScreen (宝箱画面)
│   │   ├── Background (背景画像)
│   │   ├── TreasureChest (宝箱画像)
│   │   ├── ItemDisplayCanvas (アイテム表示パネル)
│   │   │   └── ItemContainer (HorizontalLayoutGroup)
│   │   │       └── [動的生成] ItemSlot × n
│   │   └── ConfirmButton (確認ボタン)
│   │
│   └── BattleScreen (バトル画面)
│       ├── Background (背景画像)
│       ├── PlayerArea (プレイヤー表示エリア)
│       │   ├── PlayerImage (プレイヤー画像)
│       │   ├── PlayerHPBar (HPバー)
│       │   └── PlayerHPText (HP数値)
│       └── EnemyContainer (敵配置エリア)
│           └── [動的生成] EnemySlot × n
│
└── TestGameManager / TestBattleManager (テスト用 - 本番では削除)
```

---

## セットアップ手順

### ステップ1: Canvasの作成

1. Hierarchy上で右クリック
2. **UI → Canvas** を選択
3. CanvasのInspectorで以下を設定：
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler**:
     - **UI Scale Mode**: `Scale With Screen Size`
     - **Reference Resolution**: `X: 1920, Y: 1080`
     - **Screen Match Mode**: `Match Width Or Height`
     - **Match**: `0.5`

### ステップ2: TreasureScreenの作成

1. Canvasを選択 → 右クリック → **UI → Panel**
2. 名前を **"TreasureScreen"** に変更
3. RectTransform: Stretch（全画面）
   - 左上のアンカーアイコンをクリック → **Shift+Alt** を押しながら右下の **Stretch-Stretch** を選択
4. **TreasureScreen.cs** をアタッチ:
   - TreasureScreenオブジェクトを選択した状態で
   - Inspectorの **Add Component** ボタンをクリック
   - 検索欄に「TreasureScreen」と入力
   - **TreasureScreen** を選択してアタッチ

### ステップ3: Background（背景画像）の作成

1. TreasureScreenを選択 → 右クリック → **UI → Image**
2. 名前を **"Background"** に変更
3. RectTransform: Stretch（全画面）
   - Left, Top, Right, Bottom: すべて `0`

### ステップ4: TreasureChest（宝箱画像）の作成

1. TreasureScreenを選択 → 右クリック → **UI → Image**
2. 名前を **"TreasureChest"** に変更
3. RectTransform:
   - Anchors: Middle-Center
   - Pos Y: `-100`（中央やや下）
   - Width: `512`, Height: `512`

### ステップ5: ItemDisplayCanvas（アイテム表示パネル）の作成

1. TreasureScreenを選択 → 右クリック → **UI → Panel**
2. 名前を **"ItemDisplayCanvas"** に変更
3. RectTransform:
   - Anchors: Middle-Center
   - Width: `900`, Height: `500`
4. Image コンポーネント:
   - Color: 半透明（例: RGBA 0, 0, 0, 180）

### ステップ6: ItemContainer（アイテム配置用）の作成

1. ItemDisplayCanvasを選択 → 右クリック → **Create Empty**
2. 名前を **"ItemContainer"** に変更
3. RectTransform: Stretch（親パネル全体）
4. **Horizontal Layout Group** コンポーネントを追加:
   - Child Alignment: `Middle Center`
   - Spacing: `50`
   - Child Force Expand Width: チェックを外す
   - Child Force Expand Height: チェックを外す

### ステップ7: ConfirmButton（確認ボタン）の作成

1. TreasureScreenを選択 → 右クリック → **UI → Button - TextMeshPro**
   - ※「Button - TextMeshPro」で問題ありません（Unity 2021以降のデフォルト）
   - TMP Importerが表示されたら「Import TMP Essentials」をクリック
2. 名前を **"ConfirmButton"** に変更
3. RectTransform:
   - Anchors: Bottom-Center
   - Pos Y: `100`
   - Width: `300`, Height: `80`
4. 子要素の **Text (TMP)** を選択して設定:
   - Hierarchyで ConfirmButton の **▶** をクリックして展開
   - **Text (TMP)** という子オブジェクトがあるのでクリックして選択
   - Inspectorで以下を設定:
     - **Text Input欄**: `確認` と入力
     - **Font Size**: `32`
     - **Alignment**: 中央揃え（真ん中のアイコン）

### ステップ8: ItemSlotプレハブの作成

1. Hierarchyで一時的に作成:
   - Canvasを選択 → 右クリック → **Create Empty** → 名前を **"ItemSlot"**
   - ItemSlotを選択 → 右クリック → **UI → Image** を追加（名前: **ItemImage**）
   - ItemSlotを選択 → 右クリック → **UI → Text - TextMeshPro** を追加（名前: **ItemNameText**）

2. ItemSlotの設定:
   - RectTransform: Width `200`, Height `250`

3. ItemImageの設定:
   - RectTransform: Width `200`, Height `200`
   - Anchors: Top-Center
   - Pos Y: `25`（上寄せ）

4. ItemNameText（Text (TMP)）の設定:
   - RectTransform: Width `200`, Height `50`
   - Anchors: Bottom-Center
   - Pos Y: `-100`（下寄せ）
   - Font Size: `24`
   - Alignment: 中央揃え（Center）
   - Text Wrapping: Disabled（1行表示）

5. **プレハブ化**:
   - Projectウィンドウで `Assets/Prefabs/UI/` フォルダを作成（なければ）
   - HierarchyのItemSlotをProjectの `Assets/Prefabs/UI/` にドラッグ&ドロップ
   - Hierarchyから元のItemSlotを削除

### ステップ9: TreasureScreenコンポーネントの設定

TreasureScreenオブジェクトを選択し、Inspectorで以下を設定：

#### 背景設定
| フィールド | 設定 |
|-----------|------|
| Background Image | Backgroundオブジェクト |
| Forest Planet Sprite | Forest_Planet.png |
| Ice Planet Sprite | Ice_Planet.png |
| Old Empire Planet Sprite | Old_Empire_Planet.png |
| Default Background Sprite | デフォルト背景画像 |

#### 宝箱設定
| フィールド | 設定 |
|-----------|------|
| Treasure Chest Image | TreasureChestオブジェクト |
| Treasure Chest Closed Sprite | 閉じた宝箱画像 |
| Treasure Chest Open Sprite | 開いた宝箱画像 |

#### アイテム表示設定
| フィールド | 設定 |
|-----------|------|
| Item Display Canvas | ItemDisplayCanvasオブジェクト |
| Item Container | ItemContainerオブジェクト |
| Item Slot Prefab | ItemSlotプレハブ |
| Default Item Sprite | デフォルトアイテム画像 |

#### ボタン設定
| フィールド | 設定 |
|-----------|------|
| Confirm Button | ConfirmButtonオブジェクト |

---

## アイテム画像の配置

アイテム画像は `Resources` フォルダに配置してください：

```
Assets/Resources/itemphoto/
├── drill.png
├── plasmagun.png
├── Wrench.png
└── (その他アイテム画像)
```

**注意**: `Resources.Load()` で読み込むため、必ず `Resources` フォルダ内に配置

---

## プレイヤー画像の配置

プレイヤー画像は `Sprites/UI/` フォルダに配置し、Inspectorで設定してください：

```
Assets/Sprites/UI/
├── player.png
└── (その他プレイヤー関連画像)
```

**設定方法**:
1. BattleScreenオブジェクトを選択
2. Inspectorの **BattleScreen** コンポーネントで **Player Image** フィールドに `PlayerImage` オブジェクトを設定
3. `PlayerImage` オブジェクトを選択し、Inspectorの **Image** コンポーネントで **Source Image** にプレイヤー画像を設定

**注意**: 画像が設定されていない場合は、プレースホルダー（青色の四角）が表示されます

---

## 敵の画像の配置

敵の画像は `Resources/enemies/` フォルダに配置してください：

```
Assets/Resources/enemies/
├── Claw_Loader.png
├── Omni-Roid.png
├── Typhon.png
├── Welder.png
├── Fire_Ant.png
└── (その他敵画像 - 全14体)
```

**注意**: 
- `Resources.Load()` で読み込むため、必ず `Resources` フォルダ内に配置
- ファイル名は拡張子（.png）を除いた名前で管理されます
- 画像が `null` の場合は、プレースホルダー（赤色の四角）が表示されます

## 敵データファイル（enemy_L.cs）の設定

敵の情報（ID、名前、画像ファイル名など）は `enemy_L.cs` で管理されます。

**設定手順**:
1. Hierarchyのルートで右クリック → **Create Empty**
2. 名前を **"EnemyData"** に変更
3. **enemy_L.cs** をアタッチ
4. これで敵データが利用可能になります（Inspectorで設定は不要）

**使用方法**:
```csharp
// enemy_Lのインスタンスを取得
enemy_L enemyData = GetComponent<enemy_L>();
// または
enemy_L enemyData = FindObjectOfType<enemy_L>();

// 敵IDから情報を取得
string name = enemyData.GetEnemyName(0);              // "クローローダー"
string imageName = enemyData.GetEnemyImageName(0);    // "Claw_Loader"
List<int> stats = enemyData.GetEnemyData(0);          // [60, 21, 5, ...]
EnemyType type = enemyData.GetEnemyType(0);           // Common

// 画像を読み込む
Sprite sprite = Resources.Load<Sprite>($"enemies/{imageName}");

// 惑星ごとの敵を取得
List<int> forestEnemies = enemyData.GetEnemyIdsByPlanet("Forest_Planet");
// → [0, 1, 8, 9, 10] (雑魚2体 + Forest_Planetの3体)

// 惑星とタイプで絞り込み
List<int> forestBosses = enemyData.GetEnemyIdsByPlanetAndType("Forest_Planet", EnemyType.Boss);
// → [10] (Briareusのみ)
```

**敵ID一覧（0-17）**:
- 0, 1: 雑魚モブ（全惑星共通）
- 2-4: Volcano_Planet（3個目がボス）
- 5-7: Ice_Planet（3個目がボス）
- 8-10: Forest_Planet（3個目がボス）
- 11-13: Old_Empire_Planet（3個目がボス）
- 14-16: Desert_Planet（3個目がボス、未実装）
- 17: 最終ボス（未実装）

---

## 画像インポート設定

### 画像サイズについて

Unityでは画像サイズが**2段階**で処理されます：

#### 1. インポート時のリサイズ（テクスチャサイズ）

画像をUnityにインポートする際、Inspectorの**Max Size**設定に基づいてリサイズされます：

1. Projectウィンドウで画像を選択
2. Inspectorで設定:
   - **Texture Type**: `Sprite (2D and UI)`
   - **Max Size**: `2048` または `4096`（敵の画像の場合、通常は `2048` で十分）
   - **Compression**: `None` または `Normal Quality`
3. **Apply** をクリック

**注意**: 
- `Max Size`は画像の**元のサイズ**を制限します
- 例: 5000×5000pxの画像を`Max Size: 2048`に設定すると、2048×2048pxに縮小されます
- これは**テクスチャの解像度**を制限するもので、表示サイズとは別物です

#### 2. UI表示時のサイズ（RectTransform）

実際の表示サイズは**RectTransformのWidth/Height**で決まります：

- **EnemyImage**: Width `200`, Height `200` に設定済み
- 画像がどんなサイズでも、**200×200ピクセル**で表示されます
- 画像が縦長でも横長でも、RectTransformのサイズに合わせて表示されます

#### 画像のアスペクト比を保持したい場合

1. `EnemyImage`オブジェクトを選択
2. Inspectorの**Image**コンポーネントで**Preserve Aspect**にチェック
3. これで画像のアスペクト比が保持され、RectTransformのサイズ内に収まるように表示されます

#### 画像の元のサイズで表示したい場合

1. `EnemyImage`オブジェクトを選択
2. Inspectorの**Image**コンポーネントで**Set Native Size**をクリック
3. RectTransformが画像の元のピクセルサイズに自動調整されます

### 敵の画像サイズの推奨

| 項目 | 推奨値 |
|------|--------|
| 元の画像サイズ | **400×600px**（縦長、アスペクト比 2:3） |
| Max Size設定 | `2048`（通常は十分） |
| UI表示サイズ | **400×600px**（RectTransformで設定済み、画像サイズと同じ） |
| EnemySlotサイズ | 450×650px（画像を収めるためのコンテナ） |

**まとめ**:
- 画像の元のサイズ: **400×600px**（縦長）
- Unityの`Max Size`設定: `2048`（通常は十分）
- 実際の表示サイズ: **400×600px**（RectTransformで設定、画像サイズと同じ）
- **Preserve Aspect**を有効にすることで、アスペクト比が保持されます
- すべての敵（通常敵・ボス）を同じサイズで統一

### 画像サイズと表示サイズの関係

| 画像の元のサイズ | UnityのMax Size | 実際の表示サイズ |
|-----------------|----------------|----------------|
| 400×600px | 2048 | **RectTransformで決まる**（400×600px） |
| 800×1200px | 2048 | **RectTransformで決まる**（400×600px） |
| 1600×2400px | 2048 | **RectTransformで決まる**（400×600px） |

**重要なポイント**: 
- 画像を大きく作っても、**表示サイズはRectTransformの設定に従います**
- 現在の設定では、すべての敵（通常敵・ボス）を**400×600px**で統一表示します
- 画像の解像度が高いほど、拡大しても画質が劣化しにくくなります

---

## 確認チェックリスト

### TreasureScreen
- [ ] TreasureScreenオブジェクトにTreasureScreen.csがアタッチされている
- [ ] 各惑星の背景画像が設定されている
- [ ] 宝箱画像（閉/開）が設定されている
- [ ] ItemDisplayCanvasが作成されている
- [ ] ItemContainerにHorizontalLayoutGroupが設定されている
- [ ] ItemSlotプレハブが作成・設定されている
- [ ] ConfirmButtonが作成・設定されている

### Resources
- [ ] アイテム画像が `Resources/itemphoto/` に配置されている
- [ ] 敵の画像が `Resources/enemies/` に配置されている（ロジック側で読み込む）

### Sprites
- [ ] プレイヤー画像が `Sprites/UI/` に配置され、PlayerImageに設定されている

---

## ロジック側での使用例

```csharp
// GameManagerなどから（TreasureScreenを直接参照）
[SerializeField] private TreasureScreen treasureScreen;

public void EnterTreasureStage()
{
    // 惑星背景 + アイテムを表示
    treasureScreen.Show();
    List<int> itemIds = new List<int> { 0, 1, 2 };  // アイテムID
    treasureScreen.ShowTreasure("Forest_Planet", itemIds);
}

// 確認ボタンが押された時（Inspectorでイベント設定）
public void OnTreasureConfirmed()
{
    treasureScreen.Hide();
    // Map画面への遷移など
}
```

---

## 注意事項

- 画面遷移はGameManager側から制御してください
- UI側では画面遷移ロジックを含めないでください
- 仕様の詳細は `Documents/GameDesign/TreasureScreenSpec.md` を参照

---

# バトル画面（BattleScreen）セットアップ

仕様の詳細は `Documents/GameDesign/BattleScreenSpec.md` を参照

---

## BattleScreen オブジェクト階層

```
BattleScreen
├── Background (背景画像)
├── PlayerArea (プレイヤー表示エリア)
│   ├── PlayerImage (プレイヤー画像/プレースホルダー)
│   ├── PlayerHPBar (Slider)
│   └── PlayerHPText (TextMeshPro)
└── EnemyContainer (敵配置エリア - HorizontalLayoutGroup)
    └── [動的生成] EnemySlot × n
```

---

## バトル画面セットアップ手順

### ステップ1: BattleScreenの作成

1. Canvasを選択 → 右クリック → **UI → Panel**
2. 名前を **"BattleScreen"** に変更
3. RectTransform: Stretch（全画面）
4. **BattleScreen.cs** をアタッチ

### ステップ2: Background（背景画像）の作成

1. BattleScreenを選択 → 右クリック → **UI → Image**
2. 名前を **"Background"** に変更
3. RectTransform: Stretch（全画面）
   - Left, Top, Right, Bottom: すべて `0`

### ステップ3: PlayerArea（プレイヤー表示エリア）の作成

1. BattleScreenを選択 → 右クリック → **Create Empty**
2. 名前を **"PlayerArea"** に変更
3. RectTransform:
   - Anchors: Bottom-Left
   - Pos X: `250`, Pos Y: `200`
   - Width: `400`, Height: `400`

### ステップ4: PlayerImage（プレイヤー画像）の作成

1. PlayerAreaを選択 → 右クリック → **UI → Image**
2. 名前を **"PlayerImage"** に変更
3. RectTransform:
   - Anchors: Top-Center
   - Width: `300`, Height: `300`
   - Pos Y: `-50`
4. Image コンポーネント:
   - Color: 青色（プレースホルダー用）
   - ※ 画像ができたらSpriteを設定し、Colorを白に戻す

### ステップ5: PlayerHPBar（プレイヤーHPバー）の作成

1. PlayerAreaを選択 → 右クリック → **UI → Slider**
2. 名前を **"PlayerHPBar"** に変更
3. RectTransform:
   - Anchors: Bottom-Center
   - Width: `300`, Height: `30`
   - Pos Y: `30`
4. Sliderコンポーネント:
   - **Interactable**: チェックを外す（クリック不可に）
   - **Transition**: None
   - **Min Value**: `0`
   - **Max Value**: `100`
   - **Value**: `100`
5. 子オブジェクトの設定:
   - **Background**: Colorを黒に
   - **Fill Area → Fill**: Colorを赤に
   - **Handle Slide Area**: 削除（不要）

### ステップ6: PlayerHPText（HP数値）の作成

1. PlayerAreaを選択 → 右クリック → **UI → Text - TextMeshPro**
2. 名前を **"PlayerHPText"** に変更
3. RectTransform:
   - Anchors: Bottom-Center
   - Width: `300`, Height: `30`
   - Pos Y: `0`
4. TextMeshPro設定:
   - Text: `100/100`（初期値・開発用。実行時は動的に変更されるため任意）
   - Font Size: `24`
   - Alignment: 中央
   - **日本語フォントを設定**（DotGothic16-Regular SDF など）

**注意**: HPテキストは実行時に`BattleScreen.cs`から動的に更新されるため、初期値は設定しなくても動作します。ただし、エディタ上での位置確認のため、初期値（例: `100/100`）を設定しておくと便利です。

### ステップ7: EnemyContainer（敵配置エリア）の作成

1. BattleScreenを選択 → 右クリック → **Create Empty**
2. 名前を **"EnemyContainer"** に変更
3. RectTransform:
   - Anchors: Bottom-Right
   - Pivot: (1, 0)
   - Pos X: `-100`, Pos Y: `100`
   - Width: `700`, Height: `400`
4. **Horizontal Layout Group** を追加:
   - Child Alignment: `Middle Right`
   - Spacing: `50`
   - Child Force Expand Width: チェックを外す
   - Child Force Expand Height: チェックを外す

### ステップ8: EnemySlotプレハブの作成

1. Hierarchyで一時的に作成:
   - Canvasを選択 → 右クリック → **Create Empty** → 名前を **"EnemySlot"**
   - EnemySlotに **EnemySlot.cs** をアタッチ

2. EnemySlotの子要素を作成:
   ```
   EnemySlot
   ├── EnemyImage (UI → Image)
   ├── EnemyHPBar (UI → Slider)
   ├── EnemyHPText (UI → Text - TextMeshPro)
   └── EnemyNameText (UI → Text - TextMeshPro)
   ```

3. EnemySlotの設定:
   - RectTransform: Width `450`, Height `650`（400×600の画像 + 余白）

4. EnemyImageの設定:
   - RectTransform: Width `400`, Height `600`（画像の元のサイズと同じ）
   - Anchors: Top-Center
   - Color: 赤色（プレースホルダー）
   - **Image**コンポーネント:
     - **Preserve Aspect**: チェックを入れる（アスペクト比を保持）
     - ※ 画像が400×600の場合、Unity内でも400×600ピクセルで表示されます

5. EnemyHPBarの設定:
   - RectTransform: Width `350`, Height `30`（画像が大きくなったので幅を拡張）
   - Anchors: Middle-Center
   - Pos Y: `-320`（画像が400×600なので、位置を下に調整）
   - 設定はPlayerHPBarと同じ

6. EnemyHPTextの設定:
   - RectTransform: Width `350`, Height `30`
   - Pos Y: `-350`（HPBarの下）
   - Text: `50/50`（初期値・開発用。実行時は動的に変更されるため任意）
   - Font Size: `20`

7. EnemyNameTextの設定:
   - RectTransform: Width `350`, Height `30`
   - Pos Y: `-380`（HPTextの下）
   - Text: `敵の名前`
   - Font Size: `20`

8. **プレハブ化**:
   - Projectウィンドウで `Assets/Prefabs/UI/` に保存
   - Hierarchyから元のEnemySlotを削除

### ステップ9: BattleScreenコンポーネントの設定

BattleScreenオブジェクトを選択し、Inspectorで以下を設定：

#### 背景設定
| フィールド | 設定 |
|-----------|------|
| Background Image | Backgroundオブジェクト |
| Forest Planet Sprite | Forest_Planet.png |
| Ice Planet Sprite | Ice_Planet.png |
| Old Empire Planet Sprite | Old_Empire_Planet.png |

#### プレイヤー表示
| フィールド | 設定 |
|-----------|------|
| Player Image | PlayerImageオブジェクト |
| Player HP Bar | PlayerHPBarオブジェクト |
| Player HP Text | PlayerHPTextオブジェクト |
| Player Placeholder Color | 青色 |

**プレイヤー画像の設定**:
1. Hierarchyで `PlayerImage` オブジェクトを選択
2. Inspectorの **Image** コンポーネントで **Source Image** にプレイヤー画像（`Sprites/UI/player.png` など）を設定
3. 画像が設定されていない場合は、プレースホルダー（青色）が表示されます

#### 敵表示
| フィールド | 設定 |
|-----------|------|
| Enemy Container | EnemyContainerオブジェクト |
| Enemy Slot Prefab | EnemySlotプレハブ |
| Enemy Placeholder Color | 赤色 |

#### データ参照
| フィールド | 設定 |
|-----------|------|
| Enemy Data | EnemyDataオブジェクト（enemy_L.csがアタッチされているもの） |

**重要**: `Enemy Data`を設定することで、`BattleScreen.cs`が自動的に敵画像を読み込むようになります。

#### HPバー設定
| フィールド | 設定 |
|-----------|------|
| HP Bar Color | 赤色 |
| HP Bar Background Color | 黒色 |


### ステップ10: enemy_L.csの設定

1. Hierarchyのルートで右クリック → **Create Empty**
2. 名前を **"EnemyData"** に変更
3. **enemy_L.cs** をアタッチ
4. Inspectorでの設定は不要（データはコード内で定義済み）

### ステップ11: TestBattleManagerの作成（テスト用）

1. Hierarchyのルートで右クリック → **Create Empty**
2. 名前を **"TestBattleManager"** に変更
3. **TestBattleManager.cs** をアタッチ
4. Inspectorで設定:
   - **Battle Screen**: BattleScreenオブジェクト
   - **Enemy Data**: EnemyDataオブジェクト（enemy_L.csがアタッチされているもの）

---

## バトル画面 確認チェックリスト

- [ ] BattleScreenオブジェクトにBattleScreen.csがアタッチされている
- [ ] 各惑星の背景画像が設定されている
- [ ] PlayerAreaが作成され、HPバー・テキストがある
- [ ] EnemyContainerにHorizontalLayoutGroupが設定されている
- [ ] EnemySlotプレハブが作成・設定されている
- [ ] enemy_L.csがアタッチされたEnemyDataオブジェクトが作成されている
- [ ] BattleScreenコンポーネントの「Enemy Data」フィールドが設定されている
- [ ] TestBattleManagerの「Enemy Data」フィールドが設定されている
- [ ] TestBattleManagerでテスト可能

---

## テスト方法

1. Playモードを開始
2. 自動でバトル画面が表示される（autoStartOnAwake = true）
3. TestBattleManagerの右クリックメニューでテスト:
   - 「プレイヤーに10ダメージ」→ プレイヤーHPが減る
   - 「敵0に10ダメージ」→ 敵のHPが減る
   - 「惑星をIce_Planetに変更」→ 背景が変わる
   - 「バトルをリセット」→ HPがリセット

---

## ロジック側での使用例

### enemy_Lを使う方法（推奨）

```csharp
// BattleLogicなどから
[SerializeField] private enemy_L enemyData;

public void StartBattle(string planetName)
{
    // enemy_Lから敵情報を取得してEnemyDataを作成
    List<EnemyData> enemies = new List<EnemyData>();
    
    // 例：敵ID 0と1（雑魚モブ）を生成
    for (int i = 0; i < 2; i++)
    {
        int enemyId = i;
        string enemyName = enemyData.GetEnemyName(enemyId);
        List<int> stats = enemyData.GetEnemyData(enemyId);
        string imageName = enemyData.GetEnemyImageName(enemyId);
        
        // 画像を読み込む（BattleScreenでも自動読み込みされるが、事前読み込みも可能）
        Sprite sprite = Resources.Load<Sprite>($"enemies/{imageName}");
        
        enemies.Add(new EnemyData(
            enemyId.ToString(),
            enemyName,
            stats[0],  // MAXHP
            stats[0],  // 現在HP（初期値はMAXHPと同じ）
            sprite
        ));
    }
    
    // バトル開始
    battleScreen.StartBattle(planetName, enemies, playerHP, playerMaxHP);
}
```

### 画像なしで渡す方法（BattleScreenが自動読み込み）

```csharp
// BattleScreenにenemy_Lの参照を設定していれば、画像がnullでも自動で読み込まれます
List<EnemyData> enemies = new List<EnemyData>
{
    new EnemyData("0", "クローローダー", 60, 60, null),  // 画像はnull
    new EnemyData("1", "オムニロイド", 50, 50, null)    // BattleScreenが自動で読み込む
};
battleScreen.StartBattle(planetName, enemies, playerHP, playerMaxHP);
```

// プレイヤーがダメージを受けた時
public void OnPlayerDamaged(int damage)
{
    playerHP -= damage;
    battleScreen.UpdatePlayerHP(playerHP, playerMaxHP);
}

// 敵にダメージを与えた時
public void OnEnemyDamaged(int enemyIndex, int damage)
{
    enemies[enemyIndex].hp -= damage;
    battleScreen.UpdateEnemyHP(enemyIndex, enemies[enemyIndex].hp, enemies[enemyIndex].maxHP);
    
    if (enemies[enemyIndex].hp <= 0)
    {
        battleScreen.OnEnemyDefeated(enemyIndex);
    }
}
```
