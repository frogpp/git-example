SOP of Code Generator by T4 Template

1. 注意命名準則, Controller、Model、Dac 都是以 Function ID 開頭命名
	1.1. 如功能「藥商普查作業啟動」的 Function ID 即「DruggistCensusStartMain」
		1.1.1. Controller => DruggistCensusStartMainController
		1.1.2. Model => DruggistCensusStartMainModel
		1.1.3. Dac => DruggistCensusStartMainDac
2. 將 Model 加入專案, 並須確保 Model 具備以下幾個 Nested Model
	2.1. DTO
	2.2. QueryView
	2.3. EditView
3. 在每個 Model Property 上方加入必要的 Attribute, 目前支援：
	3.1. KeyAttribute: 影響 View 的 Query、Update 呈現
	3.2. RequiredAttribute: 影響 View 的 Create、Update 呈現
4. 確定以上設定沒問題後, 先進行以下操作
	4.1. 開啟 SDO/CodeTemplates/MvcView/View.include.t4 檔案, 將 assembly 來源更新為您當前專案所產生的 DLL 檔位置（參考註解）
	4.2. Rebuild 專案（確保當前專案的 DLL 檔案已包含上述設定）
5. 完成上述操作, 即開始依序執行以下 Code Generate 步驟
	5.1.【Controller】=> 於指定 Controller 目錄下點選右鍵, 執行「新增控制器 / Add Controller」
		5.1.1. 選擇 MVC 5 Controller - Empty 
		5.1.2. 輸入正確的 Controller Name（Function ID）
	5.2.【Query】於 Controller 的 Query Action 上點選右鍵, 執行「新增檢視 / Add View」, 提示視窗照下方說明填寫後即送出
		5.2.1. 檢視名稱 / View Name => 維持不變
		5.2.2. 範本 / Template => 選擇「Query(Custom)」
		5.2.3. 模型類別 / Model Class => 選擇該功能的 Model 
	5.3.【Create】=> 於 Controller 的 Create Action 上點選右鍵, 執行「新增檢視 / Add View」, 提示視窗照下方說明填寫後即送出
		5.3.1. 檢視名稱 / View Name => 維持不變
		5.3.2. 範本 / Template => 選擇「Create(Custom)」
		5.3.3. 模型類別 / Model Class => 選擇該功能的 Model 
	5.4.【Update】=> 於 Controller 的 Update Action 上點選右鍵, 執行「新增檢視 / Add View」, 提示視窗照下方說明填寫後即送出
		5.4.1. 檢視名稱 / View Name => 維持不變
		5.4.2. 範本 / Template => 選擇「Update(Custom)」
		5.4.3. 模型類別 / Model Class => 選擇該功能的 Model 
	5.5.【Dac】=> 於 Controller 的任一 Action 上點選右鍵, 執行「新增檢視 / Add View」, 提示視窗照下方說明填寫後即送出
		5.4.1. 檢視名稱 / View Name => 輸入正確的 Dac Name（Function ID）
		5.4.2. 範本 / Template => 選擇「_Dac(Custom)」
		5.4.3. 模型類別 / Model Class => 選擇該功能的 Model 
6. 以上步驟執行完畢後即完成 Controller、Dac 及相關 View 的 Generate!

※  使用上如有任何問題, 請撥打分機 11366 洽詢 Steven Tsai ※