# TreasureScreen インスペクター設定項目

## 必須設定項目

### 1. 背景設定
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Background Image** | `Image` | 背景画像を表示するImageコンポーネント | Hierarchyの`Background`オブジェクトをドラッグ&ドロップ |

### 2. 惑星背景画像
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Forest Planet Sprite** | `Sprite` | Forest_Planet用の背景画像 | `Sprites/UI/Forest_Planet.png`を設定 |
| **Ice Planet Sprite** | `Sprite` | Ice_Planet用の背景画像 | `Sprites/UI/Ice_Planet.png`を設定 |
| **Old Empire Planet Sprite** | `Sprite` | Old_Empire_Planet用の背景画像 | `Sprites/UI/Old_Empire_Planet.png`を設定 |
| **Desert Planet Sprite** | `Sprite` | Desert_Planet用の背景画像（未実装） | オプション |
| **Volcano Planet Sprite** | `Sprite` | Volcano_Planet用の背景画像（未実装） | オプション |
| **Default Background Sprite** | `Sprite` | デフォルト背景画像（惑星が不明な場合） | オプション |

### 3. 宝箱設定
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Treasure Chest Image** | `Image` | 宝箱画像を表示するImageコンポーネント | Hierarchyの`TreasureChest`オブジェクトをドラッグ&ドロップ |
| **Treasure Chest Closed Sprite** | `Sprite` | 閉じた宝箱の画像 | 宝箱の閉じた状態のスプライトを設定 |
| **Treasure Chest Open Sprite** | `Sprite` | 開いた宝箱の画像 | 宝箱の開いた状態のスプライトを設定 |

### 4. アイテム表示キャンバス
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Item Display Canvas** | `GameObject` | アイテム表示用のパネル（宝箱を覆う） | Hierarchyの`ItemDisplayCanvas`オブジェクトをドラッグ&ドロップ |
| **Item Container** | `Transform` | アイテムスロットの親オブジェクト（HorizontalLayoutGroup付き） | Hierarchyの`ItemContainer`オブジェクトをドラッグ&ドロップ |

### 5. スロットプレハブ
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Item Slot Prefab** | `GameObject` | アイテム表示用のプレハブ | `Prefabs/UI/ItemSlot.prefab`をドラッグ&ドロップ |
| **Credit Slot Prefab** | `GameObject` | お金・スキルポイント表示用のプレハブ | `Prefabs/UI/CreditSlot.prefab`をドラッグ&ドロップ（未作成の場合は`ItemSlot.prefab`にフォールバック） |

### 6. 確認ボタン
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Confirm Button** | `Button` | 確認ボタンコンポーネント | Hierarchyの`ConfirmButton`オブジェクトをドラッグ&ドロップ |

### 7. アイテムデータ参照
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **Item Data** | `item` | item.csへの参照（アイテム名取得用） | シーン内の`item`コンポーネントがアタッチされたオブジェクトをドラッグ&ドロップ |

## オプション設定項目

### 8. アイテム画像設定
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **Default Item Sprite** | `Sprite` | デフォルトのアイテム画像（画像が見つからない場合） | `null`（未設定可） |

### 9. 演出設定
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **Chest Wait Time** | `float` | 宝箱表示から開くまでの待機時間（秒） | `1.0` |
| **Chest Open Time** | `float` | 宝箱が開いてからアイテム表示までの待機時間（秒） | `0.5` |

### 10. レアリティ色設定
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **Common Text Color** | `Color` | Commonテキスト色（白） | `Color.white` |
| **Rare Text Color** | `Color` | Rareテキスト色（青） | `RGB(0, 128, 255)` |
| **Epic Text Color** | `Color` | Epicテキスト色（紫） | `RGB(128, 0, 255)` |

### 11. 表示制限
| フィールド名 | 型 | 説明 | デフォルト値 |
|------------|-----|------|------------|
| **Max Reward Display Count** | `int` | 最大表示数（3-5個） | `5` |

## イベント設定

### 12. 確認ボタンイベント
| フィールド名 | 型 | 説明 | 設定方法 |
|------------|-----|------|---------|
| **On Confirm Button Clicked** | `UnityEvent` | 確認ボタンが押された時のイベント | Inspectorのイベント欄で、呼び出したいメソッドを設定（例: `TreasureScreen.Hide()`） |

## 設定手順

1. **Hierarchy構造の確認**
   - `TreasureScreen`オブジェクトが存在する
   - `Background`、`TreasureChest`、`ItemDisplayCanvas`、`ItemContainer`、`ConfirmButton`が子オブジェクトとして存在する

2. **TreasureScreenコンポーネントの設定**
   - `TreasureScreen`オブジェクトを選択
   - Inspectorで`TreasureScreen`コンポーネントを確認
   - 上記の各フィールドに適切なオブジェクト/スプライトを設定

3. **プレハブの確認**
   - `ItemSlot.prefab`が`Prefabs/UI/`に存在する
   - `CreditSlot.prefab`が`Prefabs/UI/`に存在する（オプション、未作成の場合は`ItemSlot.prefab`にフォールバック）

4. **Resourcesフォルダの確認**
   - `Resources/itemphoto/`フォルダにアイテム画像が配置されている
   - `Resources/itemphoto/Monney_credit.png`が存在する（お金表示用）
   - `Resources/itemphoto/skill_credit.png`が存在する（スキルポイント表示用）

## 注意事項

- **Item Container**には`HorizontalLayoutGroup`コンポーネントが必要です
- **Credit Slot Prefab**が未設定の場合、`ItemSlot.prefab`が使用されます（警告ログが出力されます）
- **Item Data**が未設定の場合、アイテム名が「アイテム{ID}」と表示されます
- レアリティ色はバトルシーンと同じ色設定を使用します（変更可能）

