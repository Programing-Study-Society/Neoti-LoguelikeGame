# バトルシステム統合後のInspector設定ガイド

## 必須設定項目

### 1. `battle_system` コンポーネント

#### 基本設定（既存の設定を確認）
- **Enemy Prefab**: 敵ロジック用プレハブ（enemy_Lコンポーネント付き）
- **Enemy Spawn Manager**: 敵出現管理コンポーネント
- **Enemy Data File**: 敵データファイル（enemy_L.LoadFromDataFile用）
- **Stage Data**: ステージ進行度データ（惑星名取得用）
- **Item Master**: アイテムマスター（所持数管理用）
- **Battle Screen**: バトル画面UIコンポーネント
- **Player**: プレイヤーコンポーネント
- **Enemy Spacing**: 敵同士の間隔（デフォルト: 2.0）

#### 新規追加：アイテム効果設定（デフォルト値が設定済み）

##### Heal Settings（回復アイテム - アイテム0）
- **Heal Small Amount**: 5,000（Common）
- **Heal Middle Amount**: 8,000（Rare）
- **Heal Large Amount**: 15,000（Epic）

##### MultiHit Settings（マルチヒット - アイテム1）
- **Multi Hit Small Atk Rate**: 1.0（ATK100%）
- **Multi Hit Middle Atk Rate**: 1.5（ATK150%）
- **Multi Hit Large Atk Rate**: 3.0（ATK300%）
- **Multi Hit Min**: 1（最小ヒット数）
- **Multi Hit Max**: 6（最大ヒット数）

##### DOT Settings（持続ダメージ - アイテム3）
- **Dot Duration Turns**: 3（継続ターン数）
- **Dot Small Atk Rate**: 2.5（1ターンあたりATK250%）
- **Dot Middle Atk Rate**: 3.75（1ターンあたりATK375%）
- **Dot Large Atk Rate**: 7.5（1ターンあたりATK750%）

##### Strong Attack Settings（強攻撃 - アイテム2）
- **Strong Small Atk Rate**: 4.0（ATK400%）
- **Strong Middle Atk Rate**: 6.0（ATK600%）
- **Strong Large Atk Rate**: 12.0（ATK1200%）

##### Strong Attack Hidden Effects（強攻撃の隠し効果）
- **Strong Fail Denominator**: 20（1/20の確率で失敗）
- **Strong Fail Multiplier**: 0.5（失敗時：ダメージ50%）
- **Strong Lucky Denominator**: 30（1/30の確率で上振れ）
- **Strong Lucky Multiplier**: 3.0（上振れ時：ダメージ300%）

##### Player ATK Buff Settings（プレイヤーATKバフ - アイテム5）
- **Atk Buff Duration Turns**: 3（継続ターン数）
- **Atk Buff Small Percent**: 40（ATK+40%）
- **Atk Buff Middle Percent**: 70（ATK+70%）
- **Atk Buff Large Percent**: 100（ATK+100%）

##### Enemy DEF Debuff Settings（敵DEFデバフ - アイテム6）
- **Def Debuff Duration Turns**: 3（継続ターン数）
- **Def Debuff Small Percent**: 40（DEF-40%）
- **Def Debuff Middle Percent**: 70（DEF-70%）
- **Def Debuff Large Percent**: 100（DEF-100%）

##### Player DEF Buff Settings（プレイヤーDEFバフ - アイテム7）
- **Def Buff Duration Turns**: 3（継続ターン数）
- **Def Buff Small Percent**: 40（DEF+40%）
- **Def Buff Middle Percent**: 70（DEF+70%）
- **Def Buff Large Percent**: 100（DEF+100%）

---

### 2. `BattleScreen` コンポーネント（既存の設定を確認）

#### 基本設定
- **Background Image**: 背景画像
- **惑星背景画像**: 各惑星の背景スプライト
- **Player Image**: プレイヤー画像
- **Player HP Bar**: プレイヤーHPバー（Slider）
- **Player HP Text**: プレイヤーHPテキスト
- **Enemy Container**: 敵を配置する親オブジェクト
- **Enemy Slot Prefab**: 敵表示用プレハブ
- **Enemy Data**: enemy_L.csへの参照
- **Item Data**: item.csへの参照
- **Battle System**: battle_systemコンポーネントへの参照

#### アイテム選択パネル
- **Item Selection Panel**: アイテム選択パネル（中央表示）
- **Item Grid Container**: アイテムグリッドの親（GridLayoutGroup）
- **Battle Item Slot Prefab**: バトル用アイテムスロットプレハブ
- **Item Selection Title Text**: タイトルテキスト（オプション）

#### 攻撃ボタン（新規追加された機能）
- **Attack Button Panel**: 攻撃ボタンパネル（アイテム選択後に表示）
- **Attack Button Container**: 攻撃ボタンの親（HorizontalLayoutGroupなど）
- **Attack Button Prefab**: 攻撃ボタンプレハブ

---

## 設定の確認手順

### 1. `battle_system` オブジェクトの確認
1. Hierarchyで`battle_system`オブジェクトを選択
2. Inspectorで以下を確認：
   - ✅ 基本設定（Enemy Prefab, Enemy Spawn Manager等）が設定されているか
   - ✅ 新規追加されたアイテム効果設定のデフォルト値が表示されているか（デフォルト値で動作します）

### 2. `BattleScreen` オブジェクトの確認
1. Hierarchyで`BattleScreen`オブジェクトを選択
2. Inspectorで以下を確認：
   - ✅ 基本設定が設定されているか
   - ✅ **Attack Button Panel**が設定されているか（新規追加）
   - ✅ **Attack Button Container**が設定されているか（新規追加）
   - ✅ **Attack Button Prefab**が設定されているか（新規追加）

---

## 新規追加された設定項目の説明

### Attack Button Panel / Container / Prefab
アイテム選択後に表示される攻撃ボタン用の設定です。

- **Attack Button Panel**: 攻撃ボタンを表示するパネル（GameObject）
- **Attack Button Container**: 攻撃ボタンを配置する親オブジェクト（HorizontalLayoutGroup推奨）
- **Attack Button Prefab**: 攻撃ボタンのプレハブ（Button + TextMeshProUGUI）

#### プレハブの要件
- `Button`コンポーネントが必要
- 子オブジェクトに`TextMeshProUGUI`が必要（ボタンのテキスト表示用）

---

## 注意事項

1. **アイテム効果設定**: デフォルト値が設定されているため、基本的には変更不要です。バランス調整が必要な場合のみ変更してください。

2. **Attack Button Prefab**: 新しく作成する必要がある場合は、以下の構造を推奨：
   ```
   AttackButton (GameObject)
   ├─ Button (Button Component)
   └─ Text (TextMeshProUGUI Component)
   ```

3. **既存の設定**: 以前から設定していた項目（Enemy Prefab, Enemy Spawn Manager等）はそのまま使用できます。

---

## 設定が完了したら

1. UnityでPlayモードに入る
2. `summary`オブジェクトのInspectorメニューから「バトルを開始」を選択
3. アイテム選択→攻撃ボタン表示→アイテム使用の流れを確認

問題があれば、Consoleログを確認してください。

