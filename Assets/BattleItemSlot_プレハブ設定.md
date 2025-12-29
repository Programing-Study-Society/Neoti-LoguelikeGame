# BattleItemSlotプレハブの設定仕様

## 全体構造

```
BattleItemSlot (親、Buttonコンポーネント)
├── ItemImage (子、Imageコンポーネント)
├── ItemNameText (子、TextMeshProUGUI)
└── CountText (子、TextMeshProUGUI)
```

---

## 1. BattleItemSlot（親オブジェクト）

### コンポーネント
- **Button** コンポーネント
  - **Target Graphic**: ItemImageを設定
  - **Interactable**: チェックを入れる
  - **Transition**: Color Tint（デフォルト）

- **BattleItemSlot** スクリプト
  - **Item Image**: ItemImageオブジェクトを設定
  - **Item Name Text**: ItemNameTextオブジェクトを設定
  - **Count Text**: CountTextオブジェクトを設定
  - **Item Button**: BattleItemSlot自身を設定（または自動取得）
  - **Common Color**: 白 (R:255, G:255, B:255)
  - **Rare Color**: 青 (R:0, G:100, B:255)
  - **Epic Color**: マゼンタ (R:255, G:0, B:255)

### Rect Transform
- **Width**: `150`
- **Height**: `200`
- **Anchors**: Top-Left（デフォルト）
- **Pivot**: (0.5, 0.5)

### その他
- **Active**: チェックを入れる

---

## 2. ItemImage（子オブジェクト）

### コンポーネント
- **Image** コンポーネント
  - **Source Image**: （実行時に設定される）
  - **Color**: 白 (R:255, G:255, B:255, A:255)
  - **Preserve Aspect**: チェックを入れる（画像の縦横比を保持）
  - **Raycast Target**: チェックを入れる

### Rect Transform
- **Anchors**: Stretch-Stretch
  - **Anchor Min**: `X: 0, Y: 0`
  - **Anchor Max**: `X: 1, Y: 1`
- **Left**: `10`（左から10ピクセル）
- **Top**: `50`（上から50ピクセル、ItemNameText分の余白）
- **Right**: `10`（右から10ピクセル）
- **Bottom**: `50`（下から50ピクセル、CountText分の余白）
- **Pivot**: (0.5, 0.5)

### その他
- **Active**: チェックを入れる

---

## 3. ItemNameText（子オブジェクト）

### コンポーネント
- **TextMeshPro - Text (UI)** コンポーネント
  - **Text**: `アイテム名`（仮、実行時に設定される）
  - **Font Size**: `20`
  - **Alignment**: 中央揃え
  - **Color**: 白 (R:255, G:255, B:255, A:255)
  - **Raycast Target**: チェックを外す（クリック判定を親に任せる）

### Rect Transform
- **Anchors**: Bottom-Stretch
  - **Anchor Min**: `X: 0, Y: 0`
  - **Anchor Max**: `X: 1, Y: 0`
- **Height**: `40`
- **Pos Y**: `10`（下から10ピクセル上）
- **Left**: `0`
- **Right**: `0`
- **Pivot**: (0.5, 0.5)

### その他
- **Active**: チェックを入れる

---

## 4. CountText（子オブジェクト）

### コンポーネント
- **TextMeshPro - Text (UI)** コンポーネント
  - **Text**: `×1`（仮、実行時に設定される）
  - **Font Size**: `18`
  - **Alignment**: 右揃え
  - **Color**: 白 (R:255, G:255, B:255, A:255)
  - **Raycast Target**: チェックを外す（クリック判定を親に任せる）

### Rect Transform
- **Anchors**: Top-Stretch
  - **Anchor Min**: `X: 0, Y: 1`
  - **Anchor Max**: `X: 1, Y: 1`
- **Height**: `30`
- **Pos Y**: `-10`（上から10ピクセル下）
- **Left**: `0`
- **Right**: `0`
- **Pivot**: (0.5, 0.5)

### その他
- **Active**: チェックを入れる

---

## 設定のポイント

### 1. サイズの関係
- 親（BattleItemSlot）: 150×200
- ItemImage: 親の内側に余白を持って配置（Left:10, Top:50, Right:10, Bottom:50）
- ItemNameText: 高さ40、下から10ピクセル上
- CountText: 高さ30、上から10ピクセル下

### 2. レイアウトの考え方
```
┌─────────────────┐ ← BattleItemSlot (150×200)
│  ×2             │ ← CountText (上、右揃え)
│                 │
│                 │
│   [画像]        │ ← ItemImage (中央、余白あり)
│                 │
│                 │
│  リペアユニット │ ← ItemNameText (下、中央揃え)
└─────────────────┘
```

### 3. アンカーの使い分け
- **ItemImage**: Stretch-Stretch（親のサイズに合わせる、余白付き）
- **ItemNameText**: Bottom-Stretch（下に固定、幅は親に合わせる）
- **CountText**: Top-Stretch（上に固定、幅は親に合わせる）

---

## 確認チェックリスト

### BattleItemSlot（親）
- [ ] Width: 150, Height: 200
- [ ] Buttonコンポーネントが設定されている
- [ ] BattleItemSlotスクリプトが設定されている
- [ ] BattleItemSlotスクリプトの各フィールドに子オブジェクトが設定されている

### ItemImage
- [ ] Anchors: Stretch-Stretch
- [ ] Left: 10, Top: 50, Right: 10, Bottom: 50
- [ ] Imageコンポーネントが設定されている
- [ ] Preserve Aspect: チェックを入れる

### ItemNameText
- [ ] Anchors: Bottom-Stretch
- [ ] Height: 40, Pos Y: 10
- [ ] TextMeshProコンポーネントが設定されている
- [ ] Alignment: 中央揃え

### CountText
- [ ] Anchors: Top-Stretch
- [ ] Height: 30, Pos Y: -10
- [ ] TextMeshProコンポーネントが設定されている
- [ ] Alignment: 右揃え

---

## トラブルシューティング

### スロットが表示されない場合
1. 各子オブジェクトのActiveがONか確認
2. ItemImageのRectTransformのサイズが0でないか確認
3. BattleItemSlotスクリプトの各フィールドに子オブジェクトが設定されているか確認

### 画像が表示されない場合
1. ItemImageのImageコンポーネントのColorのAlpha値が255か確認
2. ItemImageのPreserve Aspectがチェックされているか確認
3. Resources/itemphoto/フォルダに画像ファイルがあるか確認

### テキストが表示されない場合
1. ItemNameTextとCountTextのTextMeshProコンポーネントのColorのAlpha値が255か確認
2. Font Sizeが0でないか確認
3. Textが空文字列でないか確認

