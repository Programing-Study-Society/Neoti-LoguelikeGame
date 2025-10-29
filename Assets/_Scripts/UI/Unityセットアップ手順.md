# Unity UIセットアップ手順

## オブジェクト階層構造

### 全体構造

```
Hierarchy
├── Canvas (UI全体の親)
│   ├── EventSystem (自動生成される - UI操作のため必要)
│   ├── TitleScreen (タイトル画面の親オブジェクト)
│   │   ├── BackgroundImage (背景画像)
│   │   ├── StartButton (ゲーム開始ボタン)
│   │   │   └── Text (ボタンテキスト - 自動生成)
│   │   └── OptionButton (オプションボタン)
│   │       └── Text (ボタンテキスト - 自動生成)
│   ├── MapScreen (マップ画面の親オブジェクト)
│   │   └── ScrollView (スクロールビュー)
│   │       ├── Viewport
│   │       │   └── Content
│   │       │       └── MapBackgroundImage (マップ背景画像)
│   │       └── Scrollbar Vertical (スクロールバー - オプション)
│   └── (今後追加する画面もここに追加)
│       └── OptionScreen
│       └── GameHUD
│       └── など...
└── UIManager (空のGameObject - UI管理用)
```

### コンポーネント構成図

```
Canvas
└── TitleScreen (GameObject)
    ├── TitleScreen (Script コンポーネント)
    │   ├── Background Image: BackgroundImage を参照
    │   ├── Start Button: StartButton を参照
    │   ├── Option Button: OptionButton を参照
    │   ├── Title Background Sprite: スプライト参照
    │   └── Button Sprite: スプライト参照
    │
    ├── BackgroundImage (GameObject)
    │   ├── RectTransform
    │   └── Image (コンポーネント)
    │
    ├── StartButton (GameObject)
    │   ├── RectTransform
    │   ├── Image (コンポーネント)
    │   ├── Button (コンポーネント)
    │   └── Text (GameObject - 子要素)
    │       ├── RectTransform
    │       └── Text (コンポーネント)
    │
    └── OptionButton (GameObject)
        ├── RectTransform
        ├── Image (コンポーネント)
        ├── Button (コンポーネント)
        └── Text (GameObject - 子要素)
            ├── RectTransform
            └── Text (コンポーネント)

UIManager (GameObject - Canvasの外)
└── UIManager (Script コンポーネント)
    ├── Title Screen: TitleScreen を参照
    └── Map Screen: MapScreen を参照

Canvas
└── MapScreen (GameObject)
    ├── MapScreen (Script コンポーネント)
    │   ├── Scroll Rect: ScrollView を参照
    │   ├── Map Background Image: MapBackgroundImage を参照
    │   └── Map Background Sprite: スプライト参照
    │
    └── ScrollView (GameObject)
        ├── Scroll Rect (コンポーネント)
        ├── Viewport (GameObject)
        │   ├── RectTransform
        │   └── Mask (コンポーネント)
        │       └── Image (コンポーネント)
        │
        └── Content (GameObject)
            ├── RectTransform
            └── MapBackgroundImage (GameObject)
                ├── RectTransform (サイズ: Width 1920, Height 6000)
                └── Image (コンポーネント)
```

## セットアップ手順

### ステップ1: Canvasの作成

1. Hierarchy上で右クリック
2. **UI → Canvas** を選択
3. Canvasが作成され、EventSystemも自動で作成されます
4. CanvasのInspectorで以下の設定を確認・変更：
   - **Render Mode**: `Screen Space - Overlay` (デフォルト)
   - **Canvas Scaler** コンポーネント:
     - **UI Scale Mode**: `Scale With Screen Size` に変更
     - **Reference Resolution**: `X: 1920, Y: 1080` (推奨)
     - **Screen Match Mode**: `Match Width Or Height`
     - **Match**: `0.5` (中央)

### ステップ2: TitleScreenオブジェクトの作成

1. Canvasを選択した状態で右クリック
2. **Create Empty** を選択（または `Ctrl+Shift+N` / Mac: `Cmd+Shift+N`）
3. 名前を **"TitleScreen"** に変更
4. TitleScreenオブジェクトに **TitleScreen** コンポーネントをアタッチ：
   - Inspectorの **Add Component** をクリック
   - "TitleScreen" と検索して選択

### ステップ3: BackgroundImageの作成

1. TitleScreenオブジェクトを選択した状態で右クリック
2. **UI → Image** を選択
3. 名前を **"BackgroundImage"** に変更
4. RectTransformの設定：
   - 左上の **アンカー** をクリック
   - **Shift+Alt+クリック** で **Stretch - Stretch** を選択（画面全体に拡張）
   - **Left, Top, Right, Bottom** を全て `0` に設定
5. Imageコンポーネントの設定：
   - **Image Type**: `Simple`
   - **Preserve Aspect**: チェックを外す（背景は画面全体に広げるため）

### ステップ4: StartButtonの作成

1. TitleScreenオブジェクトを選択した状態で右クリック
2. **UI → Button** を選択
3. 名前を **"StartButton"** に変更
4. RectTransformの設定：
   - **Pos X**: `0`
   - **Pos Y**: `-150` (画面下部寄りに配置、調整可)
   - **Width**: `200`
   - **Height**: `60`
5. Buttonコンポーネントはデフォルトのまま
6. 子要素のTextを選択：
   - **Text** コンポーネントの **Text** フィールドは空にしておく（スクリプトで設定します）
   - **Font Size**: `24` (推奨)
   - **Alignment**: 中央揃え

### ステップ5: OptionButtonの作成

1. TitleScreenオブジェクトを選択した状態で右クリック
2. **UI → Button** を選択
3. 名前を **"OptionButton"** に変更
4. RectTransformの設定：
   - **Pos X**: `0`
   - **Pos Y**: `-250` (StartButtonの下に配置、調整可)
   - **Width**: `200`
   - **Height**: `60`
5. Buttonコンポーネントはデフォルトのまま
6. 子要素のTextを選択：
   - **Text** コンポーネントの **Text** フィールドは空にしておく
   - **Font Size**: `24` (推奨)
   - **Alignment**: 中央揃え

### ステップ6: TitleScreenコンポーネントの設定

1. TitleScreenオブジェクトを選択
2. InspectorのTitleScreenコンポーネントで以下を設定：
   - **Background Image**: BackgroundImageオブジェクトをドラッグ＆ドロップ
   - **Start Button**: StartButtonオブジェクトをドラッグ＆ドロップ
   - **Option Button**: OptionButtonオブジェクトをドラッグ＆ドロップ
   - **Title Background Sprite**: `Assets/Sprites/UI/ui_title_background.png` をドラッグ＆ドロップ（画像準備後に設定）
   - **Button Sprite**: `Assets/Sprites/UI/ui_button_normal.png` をドラッグ＆ドロップ（画像準備後に設定）

### ステップ7: MapScreenの作成

1. Canvasを選択した状態で右クリック
2. **UI → Scroll View** を選択
3. 作成されたScrollViewの親オブジェクトの名前を **"MapScreen"** に変更
   - 注意: ScrollViewを選択するのではなく、その親オブジェクトを選択してください
4. MapScreenオブジェクトに **MapScreen** コンポーネントをアタッチ：
   - Inspectorの **Add Component** をクリック
   - "MapScreen" と検索して選択

### ステップ8: MapScreenの構成設定

1. **ScrollViewの設定**
   - MapScreenの子要素としてScrollViewが作成されていることを確認
   - ScrollViewを選択してInspectorを確認
   - ScrollRectコンポーネント：
     - **Vertical**: チェックを入れる（縦スクロールを有効化）
     - **Horizontal**: チェックを外す（横スクロールを無効化）
     - **Viewport**: Viewportオブジェクトをドラッグ＆ドロップ
     - **Content**: Contentオブジェクトをドラッグ＆ドロップ

2. **Contentの設定**
   - ScrollViewの子要素のContentを選択
   - RectTransformの設定：
     - **Width**: `1920`（画像の幅に合わせる）
     - **Height**: `6000`（画像の高さに合わせる）
   - **Vertical Layout Group** コンポーネントを削除（もしあれば）
   - **Content Size Fitter** コンポーネントがあれば削除

3. **Viewportの設定**
   - ScrollViewの子要素のViewportを選択
   - RectTransformの設定（画面サイズに合わせる）：
     - アンカープリセットを使って画面全体に拡張（Stretch-Stretch）
     - または **Left, Top, Right, Bottom** を全て `0` に設定
   - Maskコンポーネント：
     - **Show Mask Graphic**: チェックを外す（推奨）

4. **MapBackgroundImageの作成**
   - Contentオブジェクトを選択した状態で右クリック
   - **UI → Image** を選択
   - 名前を **"MapBackgroundImage"** に変更
   - RectTransformの設定：
     - **Width**: `1920`
     - **Height**: `6000`（画像のサイズに合わせて調整）
     - **Pos X**: `0`
     - **Pos Y**: `0`
     - アンカーは左上（Top-Left）を設定
   - Imageコンポーネントの設定：
     - **Image Type**: `Simple`
     - **Preserve Aspect**: チェックを外す

5. **ScrollView内の不要な要素の削除**
   - ScrollViewの子要素で、**Scrollbar Horizontal** があれば削除（横スクロールバーは不要）
   - **Scrollbar Vertical** は残しておいても削除してもどちらでも可（今回は不要とします）

### ステップ9: MapScreenコンポーネントの設定

1. MapScreenオブジェクトを選択
2. InspectorのMapScreenコンポーネントで以下を設定：
   - **Scroll Rect**: ScrollViewオブジェクトをドラッグ＆ドロップ
   - **Map Background Image**: MapBackgroundImageオブジェクトをドラッグ＆ドロップ
   - **Map Background Sprite**: `Assets/Sprites/UI/ui_map_background.png` をドラッグ＆ドロップ（画像準備後に設定）

### ステップ10: UIManagerの作成と設定

1. Hierarchyのルート（Canvasの外）で右クリック
2. **Create Empty** を選択
3. 名前を **"UIManager"** に変更
4. UIManagerオブジェクトに **UIManager** コンポーネントをアタッチ
5. InspectorのUIManagerコンポーネントで以下を設定：
   - **Title Screen**: TitleScreenオブジェクトをドラッグ＆ドロップ
   - **Map Screen**: MapScreenオブジェクトをドラッグ＆ドロップ

## 確認事項

### タイトル画面
- [ ] Canvasが作成され、適切に設定されている
- [ ] EventSystemが自動生成されている
- [ ] TitleScreenオブジェクトにTitleScreenコンポーネントがアタッチされている
- [ ] BackgroundImageが画面全体に広がるように設定されている
- [ ] StartButtonとOptionButtonが適切な位置に配置されている
- [ ] TitleScreenコンポーネントの各フィールドが正しく設定されている

### マップ画面
- [ ] MapScreenオブジェクトが作成され、MapScreenコンポーネントがアタッチされている
- [ ] ScrollViewのScrollRectが正しく設定されている（Vertical有効、Horizontal無効）
- [ ] Contentのサイズが画像サイズ（1920*6000）に合わせられている
- [ ] Viewportが画面全体に広がるように設定されている
- [ ] MapBackgroundImageがContentの子要素として正しく配置されている
- [ ] MapBackgroundImageのサイズが画像サイズに合わせられている
- [ ] MapScreenコンポーネントの各フィールドが正しく設定されている

### UIManager
- [ ] UIManagerオブジェクトにUIManagerコンポーネントがアタッチされている
- [ ] UIManagerのTitle ScreenフィールドにTitleScreenオブジェクトが設定されている
- [ ] UIManagerのMap ScreenフィールドにMapScreenオブジェクトが設定されている

## GameManager側での設定（ロジック担当）

**重要**: UI側では画面遷移ロジックを持たず、外部（GameManagerなど）から指示を受けて表示します。

### GameManagerでの実装例

1. GameManager（仮）をHierarchyに作成
2. GameManagerコンポーネントにUIManagerの参照を保持
3. TitleScreenのイベントを購読：

```csharp
// 例（GameManager側のコード）
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TitleScreen titleScreen;

    private void Start()
    {
        // TitleScreenのイベントを購読
        if (titleScreen != null)
        {
            // UnityEventをInspectorで設定するか、コードで設定
        }
    }

    // ゲーム開始ボタンがクリックされた時に呼ばれる（UnityEventから呼ばれる）
    public void OnGameStartButtonClicked()
    {
        // タイトル画面を非表示
        uiManager?.HideTitleScreen();
        // マップ画面を表示
        uiManager?.ShowMapScreen();
    }
}
```

### Unity側でのイベント設定

1. TitleScreenオブジェクトを選択
2. InspectorのTitleScreenコンポーネントで **On Start Button Clicked** イベントを展開
3. **+** ボタンをクリックしてイベントリスナーを追加
4. GameManagerオブジェクトをドラッグ＆ドロップ
5. ドロップダウンから **GameManager → OnGameStartButtonClicked** を選択

これにより、UI側は画面遷移のトリガーを出すだけで、実際の遷移ロジックはGameManager側で制御されます。

## 注意事項

- 画像ファイル（`.png`）は、Unityエディタでインポートした後、Inspectorで **Texture Type** を **Sprite (2D and UI)** に変更してください
- ボタンのRectTransformの位置は、実際のレイアウトに応じて調整してください
- フォントサイズや色は、ゲームのデザインに合わせて調整してください
- マップ背景画像は、縦長の画像（例: 1920*6000）を用意してください
- マップ画像のサイズは、ContentとMapBackgroundImageのRectTransformで設定したサイズと一致させる必要があります
- スクロールはマウスホイールまたはドラッグで行えます
- Contentのサイズが画像サイズより小さい場合、画像が途切れて表示されます
- **画面遷移はGameManager側から制御してください**。UI側では画面遷移ロジックを含めないようにしてください

