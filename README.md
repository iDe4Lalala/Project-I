# Project-I

## シーン説明

### 1. StartScene
  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 215706" src="https://github.com/user-attachments/assets/146e01e9-c253-44d8-809d-77fc72244008" />
  ゲームが起動した時に最初に表示される画面です。背景として動画が流れています。
  「Map Select」ボタンを押すと、次のシーン(MapSelectScene)に遷移します。

### 2. MapSelectScene
  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 215740" src="https://github.com/user-attachments/assets/5e4f0d7f-201f-4923-8485-fbe89f1ac95a" />
  戦うマップを選択する画面です。現時点では1Map実装しています。
  「Select」ボタンを押すと、次のシーン(WeaponSelectScene)に遷移します。
  
### 3. WeaponSelectScene
  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 215807" src="https://github.com/user-attachments/assets/057452af-e008-434d-9e6a-6e8720b4d603" />
  使用する武器を選択する画面です。現時点ではM4の1つを実装しています。
  「Select」ボタンを押すと、次のシーン(BattleScene)に遷移します。
  
### 4. BattleScene
  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 215826" src="https://github.com/user-attachments/assets/d1fc4e0d-38eb-44d7-b2ae-013e8f55b61b" />
  2と3で選択したMap、武器で実際に敵と戦う画面です。Wave制で敵が一定数スポーンし、それをひたすら倒していくものとなっています。
  この画面に遷移したら、3カウント後に敵がスポーンするようになっています。
  Wave中にスポーンした敵を全滅させることで次のWaveに移ります。合計3つのWaveがあります。<br><br>

  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 220109" src="https://github.com/user-attachments/assets/684fd657-bd33-4e9f-90c7-48ba69da20fa" />
  敵を倒す、または敵に倒されると画面下部にキルログが表示されます。
  また、Waveが始まると「〇〇 Wave Start」、Waveの敵が全て倒されると「〇〇 Wave Clear」とそれぞれ表示されます。
  画面右下には、LP(Life Point：残機)、HP、Ammo(残弾)が表示されています。<br><br>

  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 220135" src="https://github.com/user-attachments/assets/8ba9154b-b1c8-48ff-9ca2-bd590c7e37ba" />
  プレイヤーが敵に倒されると、カメラが俯瞰視点になり、一定時間後にスポーンします。
  LPが0になると、これ以上復活しなくなります。(倒されたらゲームオーバー)<br><br>

### 5. ResultScene
  <img width="1920" height="1080" alt="スクリーンショット 2026-03-31 220147" src="https://github.com/user-attachments/assets/fe3f00f2-cb2e-4847-bd0c-93455fade7fc" />
  ゲームが終了した後に表示される画面です。ゲームクリアであれば「Game Clear」、ゲームオーバーであれば「Game Over」とそれぞれ表示されます。
  「Return To Title」ボタンを押すと、StartSceneに戻ります。

## ゲームの終了条件
  * 勝利条件
    1. 画面上部のタイマーが0になるまでに、3つのWaveの敵を全て倒す
  * 敗北条件
    1. 画面上部のタイマーが0になる
    2. LP(Life Point)が0になった後に、HPが0になる

## 遊び方
  * 対応OS：Windows
  * 使用可能デバイス：キーボードとマウス
  * 右のURLを開き、フォルダ内にあるすべてのファイルをダウンロードすることで遊べます。(OneDrive：[ダウンロードページ](https://kadai-my.sharepoint.com/:f:/r/personal/k7687735_kadai_jp/Documents/Project-I?csf=1&web=1&e=NVWYBs))

## 操作方法
| 操作 | キー |
|:---:|:-----:|
| 前進 | W |
| 左移動 | A |
| 後退 | S |
| 右移動 | D |
| ジャンプ | Space |
| ダッシュ | 左Shift |
| リロード | R |
| 視点移動 | マウス |
| 射撃 | 左クリック |

## アピールポイント
個人制作ながら、設計・実装・UI調整まで全工程を担当し、完成させました。使用や挙動設計を整理して、プレイヤーの操作が意図通りに反映される体験を作りました。
移動や射撃の挙動などFPS特有の操作感を、数値調整を行うことで安定させました。体験を構造としてとらえ、設計意図と実装を結びつける力を培いました。

## 苦労・工夫した点
苦労した点は、移動や射撃の操作感を自然にするのに時間がかかったことと、FPSゲームとしての当たり判定やカメラ制御などの挙動調整が難しかったです。
工夫した点は、開発当初は銃の反動を実装しない予定でいましたが、友人にテストプレイしてもらった際に、リコイルがあった方が良いというフィードバックをもらい、実装しました。
リコイル実装後に私自身もプレイし、強すぎない程度のリコイルをつけた方が楽しめると判断しました。

## 改善計画
  * 設定画面の実装し、感度などの調整をプレイヤー側ができるようにすることで遊びやすくする
  * MapやWeaponを追加し、選択項目に応じて切り替えることで、ゲームの幅を広くする
  * 敵AIの行動パターンを増やし、戦略性を向上させる

## 制作期間
約4ヶ月

## 使用ツールなど
  * ゲームエンジン：Unity(6000.3.9f1)
  * エディタ：VSCode
  * バージョン管理ツール：GitHub

## コード構成
| Script名 | 用途・役割 |
|:---|:-----|
| StartSceneManager | シーン遷移(MapSelectSceneへ) |
| MapSelectSceneManager | シーン遷移(WeaponSelectSceneへ) |
| WeaponSelectSceneManager | シーン遷移(BattleSceneへ) |
| ResultSceneManager | シーン遷移(StartSceneへ) |
| BattleSceneManager | BattleSceneに必要なクラス等の生成、プレイヤーが死んだ時の処理、シーン遷移(ResultSceneへ) |
| PlayerUIManager | LP、HP、Ammo、CrossHairの表示、処理 |
| BattleUIManager | タイマー、テキストUIの表示、処理 |
| BattleStateMachine | 各State(BattleCountdown, Wave, WaveClear, End)の生成・管理 |
| BattleCountdownState | バトル開始前のカウントダウンとプレイヤーの生成 |
| WaveState | 敵の生成、敵の残数判定、終了条件の受け取り |
| WaveClearState | UI表示とバトル終了判定 |
| BattleEndState | UI表示とバトル終了のフラグ |
| PlayerManager | プレイヤーの各操作の開始フラグ、被ダメージ判定 |
| PlayerAnimatonController | プレイヤーの移動、ジャンプのアニメーション実行 |
| PlayerAudioController | プレイヤーの足音の再生 |
| PlayerInputController | プレイヤーの各操作の実行 |
| PlayerInputHandler | InputSystemとPlayerManagerの橋渡し |
| EnemyManager | 敵AI(放浪、追跡、射撃)の実行、被ダメージ判定 |
| EnemyAnimationController | 敵の移動アニメーションの実行 |
| EnemyAudioController | 敵の足音を再生 |
| RifleManager | 射撃可能かの判定、射撃の実行 |
| ViewRifleAnimationManager | プレイヤー用ViewRifleのアニメーション実行(リロード) |
