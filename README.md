# EmotionHunterDEMO


#Emotion Hunter：End of Hopeless-Public Technical Showcase
Project details: <br/>https://kazarimne.github.io/kazarimneweb/<br/><br/>
Github has limited space only host C#, other files: <br/>https://drive.google.com/drive/folders/1DeybHSawuatHVm6DFRoALjc0sa2SpLwk

Purpose:
The purpose of this documentation is to provide a public technical feasibility and implementation. All of C# source code, illustration, animation, music is for my solo project " Emotion Hunter," developed independently by KazaRimne using the Unity 2022.

Notice: This is a public technical showcase. To protect the intellectual property of "Emotion Hunter," installation, runtime support, and full source are NOT provided.
<br/>
<br/>
1. Enemy AI (ATK_flower.cs) :
Line-of-Sight (Raycasting): Intelligent detection preventing "wall-clipping" attacks.
Trigonometric Tracking: Math-based (cosin) smooth rotation and projectile trajectory.
<br/>

2. Melee Enemy Movement AI (Monstermove.cs
Monster Movement & Chasing Mechanics:
Patrolling & Anti-Stuck System: Automatically changes moving direction at random intervals. It utilizes raycasts to detect walls and ledges (cliffs), effectively preventing the monster from getting stuck or falling off platforms.
Player Chasing: Uses a forward-facing raycast to lock onto the player, triggering the aggressive state (ATKing) to close distance and pursue the target.

3. Player State Machine (FSM)/(SANAState.cs): 
State-Driven Logic: Decoupled states (Move, Jump, ATK, Climb) for clean and scalable player behavior.
Complex Animation Handling: Integrated directional switching and frame-rate control via a unified interface. 
Combo & Skill Execution: Precise timing management for multi-stage attacks and state transitions. 
<br/>

4. Player Controller(Movesana.cs):
Jump Buffer & Gravity Control: Implemented Jump Buffering and dynamic gravity scaling (Low/High). 
Wall-Interaction Logic: Custom wall-climbing and "Top-Edge" detection that automatically boosts the player over obstacles.
State-Synced Movement: Deeply integrated with the FSM to manage dash (Shift), stamina consumption, and directional scaling.
<br/>

5. Player Attack (Attacksana.cs): 
Implements a multi-stage, on-cooldown combo system using an int Count variable and IEnumerator coroutines. 
Each stage features its own independent cooldown and buffering window; if no subsequent inputs are received within the timeframe, the combo chain resets. 
Upon each attack, it plays a corresponding sword slash visual effect based on the current combo stage, then hides the renderer to prevent redundant memory re-reads. Simultaneously, it dynamically instantiates a no image sword hitbox prefab in front of the character, which is automatically destroyed shortly after.
<br/>

6. Hit Effect Generation (SwordsanaOne.cs)
Branching Damage & Camera Shake: Upon detecting an enemy, it calculates random damage based on a multiplier tied to the current attack state and triggers varying intensities of screen shake.
Hit Stop via Coroutines: Temporarily sets the enemy's velocity to zero and locks their position through the DoHitStop coroutine to create a impactful sense of combat feedback.
Object Pool Management: Performance is optimized by fetching specific hit effects (such as explosions, slashes, energy balls, and dust) from an ObjectPool. It dynamically adjusts their positions and rotations before safely recycling them via a countdown timer.
<br/>

7. Dynamic Sword VFX (Flash0.cs):
Shader Property Control: Utilizes DOTween to manipulate Material properties (_Fill_Amount) in real-time.
Object-Level Optimization: Implemented renderer-culling and material instantiation management for better resource efficiency. 
<br/>

8. Custom Terrain/Background Shader (Yuka_one.shader):
Procedural Randomization: Implemented a Hash-based grid system for random 90-degree rotations and horizontal flips to break texture tiling patterns. 
Seamless Sampling: Utilized ddx/ddy and tex2Dgrad to resolve coordinate derivative issues, ensuring seamless texture transitions without artifacts. 
<br/>

9. Aiming and Locking System (Mouseca.cs)
Mouse Tracking: Determines whether the mouse cursor is located on the left or right side of the player.
Right-Click Manual Aiming: While holding down the right mouse button, the system calculates and records the firing angle and direction vector pointing toward the mouse position.
Click to Lock-On Enemy: Right-clicking an enemy locks onto or unlocks them. Upon lock-on, a targeting visual effect is generated and follows the enemy; the lock-on automatically breaks if the enemy moves out of range.

<br/>

10. Camera Control System (Cameracol.cs)
Dual Tracking Mode Toggle:
Pressing the Spacebar toggles between two modes:
Centered Tracking: Forces the camera to smoothly position and center itself directly above the player.
Bounding Box Free Movement: Allows the player to move freely within a specific inner region of the screen. The camera only starts moving when the player touches the designated edge thresholds (using SmoothDamp to eliminate jitter).
Camera Shake:
Generates a screen shake effect based on external parameters passed into the TriggerShake function.


<br/>
<br/>
<br/>
<br/>
<br/>
#Emotion Hunter：願無末路 開源代碼展示
遊戲詳情: <br/>https://kazarimne.github.io/kazarimneweb/<br/><br/>
Github空間有限只能放C#其他檔案放在: <br/>https://drive.google.com/drive/folders/1DeybHSawuatHVm6DFRoALjc0sa2SpLwk

此檔案目的為展示遊戲程序可行性與計畫專案的公開展示。所有的C#編程、插畫、動畫、音樂等皆由我本人一人創於，使用Unity 2022開發我的獨立遊戲Emotion Hunter。
此為技術展示，為了確保智慧財產權僅展示部分，也不提供任何使用方法。
<br/>
<br/>

1.砲台敵人AI (ATK_flower.cs) :
視線偵測： 具備射線判定，防止穿牆攻擊。
三角函數追蹤： 基於數學（餘弦cos）的旋轉與彈道運算。
<br/>

2 .近戰敵人移動AI(Monstermove.cs):
怪物移動與追擊機制：
巡邏與防卡牆：隨機時間自動轉向，並透過射線檢測牆壁與懸崖（邊緣），避免怪物卡住或掉落。
玩家追擊：用前方射線鎖定玩家，觸發攻擊狀態（ATKing）並朝玩家方向移動、逼近。
<br/>

3. FSM狀態機(SANAState.cs): 
狀態驅動邏輯：將動作（移動、跳躍、攻擊、爬牆）解耦為獨立狀態，確保行為與偵數控制。
複合動畫處理：統一管理方向判定與動畫幀率，支援順向與逆向（如爬牆）播放邏輯。
連擊與技能執行：精確控制多段攻擊的判定時間與狀態轉換銜接。
<br/>

4.玩家控制(Movesana.cs):
跳躍緩衝與重力控制： 實作跳躍預輸入緩衝與動態重力縮放（高/低重力），確保平台跳躍的靈敏度與手感。
牆面互動邏輯： 自定義爬牆與「牆頂邊緣偵測」，實現爬牆跳躍等流暢動作。
狀態同步運動： 與狀態機深度整合，管理衝刺、耐力消耗及方向縮放。
<br/>

5. 玩家攻擊(Attacksana.cs):
用int Count與IEnumerator實現有冷卻和階段的攻擊。
每段都有獨立的冷卻時間與緩衝機制，若太久沒點擊會重置連段。
每次攻擊時，會根據目前段數播放對應的刀光特效後隱藏renderer避免記憶體重複讀取，並在角色前方動態生成無圖片的劍判定預製體，隨後自動銷毀。
<br/>

6.攻擊特效生成(SwordsanaOne.cs)
分歧傷害與震屏：當偵測到敵人時，會根據目前攻擊狀態給予不同倍率的隨機傷害，並觸發不同強度的畫面劇烈震動。
透過協程（DoHitStop）短暫將敵人的速度歸零並固定，營造出戰鬥的打擊感與頓挫感。
特效物件池管理：從優化效能的物件池（ObjectPool）中取特定擊中特效（如爆炸、刀光、光球、煙塵），調整特效的生成位置與朝向後，再透過倒數計時安全回收。
<br/>


7. 動態斬擊特效VFX (Flash0.cs):
Shader屬性控制： 利用 DOTween 即時操作材質屬性，實現流暢的戰鬥視覺回饋。
物件等級優化： 控制渲染器剔除材質實例化管理，提升資源使用效率。
<br/>

8. 地板背景Shader (Yuka_one.shader):
程序化隨機化： 實作基於 Hash 的網格系統，透過隨機旋轉與翻轉打破貼圖重複感。
無縫採樣： 利用 ddx/ddy 與 tex2Dgrad 解決坐標問題，確保貼圖變換時無黑線接縫。
<br/>

9. 瞄準與鎖定系統(Mouseca.cs):
滑鼠追蹤：判斷滑鼠在玩家左側或右側。
右鍵手動瞄準：按住右鍵時，計算並記錄指向滑鼠位置的發射角度與方向。
點擊鎖定敵人：右鍵點擊敵人可鎖定/取消鎖定，生成鎖定特效並跟隨；超出距離會自動解除。
<br/>

10. 攝影機控制系統(Cameracol.cs):
切換兩種追蹤模式：
按下空白鍵切換跟隨的正中心位置。或邊界自由移動，玩家可以在畫面中央的特定範圍內自由走動，當玩家觸碰到畫面邊緣臨界點時，鏡頭才會移動（使用 SmoothDamp 避免抖動）。
畫面震動：
依照外部對TriggerShake的輸入值，產生畫面搖晃特效。

<br/>
<br/>
<br/>
<br/>
<br/>

#Emotion Hunter：絶望のさなかに幸あれ オープンソース
ゲームについては:<br/>https://kazarimne.github.io/kazarimneweb/<br/><br/>
c#以外ののファイル大きすぎてGithubにはアップロードできないため: <br/>https://drive.google.com/drive/folders/1DeybHSawuatHVm6DFRoALjc0sa2SpLwk

本ファイルはゲームプロジェクトの実現可能性を展示するためのみ、知的財産権の保護のためにすべてのファイルは展示しません。全部のC#コーディング、イラスト、アニメーション、音楽などすべては飾燐音(KazaRimne)の自作作品です。
技術と実現可能性の展示のため、利用方法の説明はございません。
<br/>
<br/>


1. タレットエネミーAI (ATK_flower.cs) :
レイディテクト： レイによる射線判定、壁越しの攻撃を防ぎます。
三角関数による追跡：cosに基づく射撃経路とオブジェクトの回転。
<br/>

2. 近接エネミーAI (Monstermove.cs):
敵の移動とプレイヤーロックオン：
ロックオンなしはランダムで向きを変えます、射線で生き止まりと崖（道なし）を検査する、自動で向きを変え止まります、敵が動けないと落ちることを防ぎます。長い射線でプレイヤーが目の前にあるかどうかを判定し、ロックオンして追跡で攻撃します。
<br/>

3. FSMステータス(SANAState.cs): 
FSMステータスの動ぎロジック：動ぎ（移動、ジャンプ、攻撃、壁登りなど）独立のステータスに設定し：確実に動ぎやfpsのコントロールを実現します。
連撃とスキルのエクスキュート：攻撃のコンボやスキルの判定時間のチェンジを実現します。
<br/>

4.プレイヤーコントロール(Movesana.cs):
ジャンプ緩衝と重力制御：ジャンプ判定ミス防ぎと重力変えの機能でジャンプの判定性とプレイヤーコントロール感を高めます。
壁との接触ロジック：壁登りと壁のトップ位置の判定によって、壁登りと壁ジャンプの機能を実現します。
ステータスとの同期性：FSMステータスとの状態を一致し、ダッシュ、スタミナ、キャラ向きの機能を実現します。
<br/>

5. プレイヤーアタック(Attacksana.cs):
int CountとIEnumeratorを使ってカウントダウンと段階ありの連打を実現します。単独のタイマーとクールダウンがある、一定時間超えればCountがリセットします。
攻撃の時に対応するフラシュプレイした後にrendererを隠しメモリ使いを控えます、そして攻撃範囲に画像無しの判定オブジェクトを生成し消去します。
<br/>

6. アタックエフェクトジェネレート (SwordsanaOne.cs)
ダメージ判定と画面揺れ。
敵に当たると判定した時に今の攻撃状態と倍率によるランダムダメージを与える同時に、対応するエフェクトも生成して対応する画面揺れも発生します。
IEnumerator DoHitStopで一時的に敵のRigidbody速度をゼロにしてそして位置を固定し、これで打撃がある戦闘を実現します。
エフェクトオブジェクトプール：ObjectPoolを使って対応するエフェクト（バースト、ボール、破片など）を取り出して生成位置を特定して、向きを決めて生成します、後からタイマーで回収します。
<br/>

7. ダイナミック攻撃エフェクトVFX (Flash0.cs):
シェーダープロパティコントロール： DOTweenを使いやプロパティ変えでインスタントなコントロールを実現します。
オブジェクトのレベルコントロール：シェーダーでプロパティ実体を変え、より良いパフォーマンスのになれます。
<br/>

8. 地面グラウンドシェーダー (Yuka_one.shader):
プログラミングでランダム化：ハッシュ関数による計算で回転と左右反転によってミクス感あるグラウンドを実現します。
接触面サンプリング：ddx/ddy と tex2Dgrad 座標ロケーション計算し画像変換も黒い線を削除します。
<br/>

9. 狙いとロックオンシステム(Mouseca.cs):
カーソル追跡：カーソルはプレイヤーの右側と左側にあるかを判定します。
右クリックで狙う：右クリックが押したままはカーソルをロックオンします。
右クリックでロックオン：右クリックでてきをロックオンとアンロックオンできます、一定距離が超えればロックオンは自動的解除します。
<br/>

10. カメラコントロールシステム(Cameracol.cs):
カメラ追跡モードは二つあります、スペースで真ん中と画面範囲内モードが変えます、カメラ移動はSmoothDampを使い揺れを防ぎます。外部からはTriggerShakeに値を値すると揺れが発生します。
<br/>
<br/>
<br/>

