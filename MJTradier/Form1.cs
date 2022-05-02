using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

// ========================================================================
// 철학 : Simple is the best.
// ========================================================================
namespace MJTradier
{ 
    public partial class Form1 : Form
    {
        

        // ------------------------------------------------------
        // 상수 변수
        // ------------------------------------------------------
        private const byte KOSDAQ_ID = 0;  // 코스닥을 증명하는 상수
        private const byte KOSPI_ID = 1; // 코스피를 증명하는 상수
        private const int BILLION = 100000000; // 1억
        private const int MAX_STOCK_NUM = 1000000; // 최종주식종목 수
        public const int NUM_SEP_PER_SCREEN = 100; // 한 화면번호 당 가능요청 수
        public const double STOCK_TAX = 0.0023; // 거래세 //++
        public const double STOCK_FEE = 0.00015; // 증권 매매 수수료
        public const double VIRTUAL_STOCK_FEE = 0.0035; // 증권 매매 수수료
        public const int MAX_STOCK_HOLDINGS_NUM = 200; // 보유주식을 저장하는 구조체 최대 갯수
        public const int EYES_CLOSE_NUM = 3; // 현재가에서 EYES_CLOSE_NUM 스텝만큼 가격을 올려 지정가에 두기 위한 스텝 변수
        public const int SHUTDOWN_TIME = 150000; // 마감시간
        public const int BAN_BUY_TIME = 144000; // 매수 종료시간
        public const int IGNORE_REQ_SEC = 10; // 요청무시용 seconds 변수

        // ------------------------------------------------------
        // 각 종목 구조체 변수
        // ------------------------------------------------------
        private int nEachStockIdx; // 개인구조체의 인덱스를 설정하기 위한 변수 0부터 시작
        private int[] eachStockIdxArray = new int[MAX_STOCK_NUM]; // 개인구조체의 인덱스를 저장한 배열
        EachStock[] eachStockArray;  // 각 주식이 가지는 실시간용 구조체(개인구조체)
        public int nCurIdx; // 현재 개인구조체의 인덱스
        
        // ------------------------------------------------------
        // 기타 변수
        // ------------------------------------------------------
        public char[] charsToTrim = { '+', '-', ' ' }; 
        public bool isDepositSet; // 예수금이 세팅돼있는 지 확인하는 변수
        // ------------------------------------------------------
        // 스크린번호 변수
        // ------------------------------------------------------
        public const int TRADE_SCREEN_NUM_START = 2000; // 매매 시작화면번호 
        public const int TRADE_SCREEN_NUM_END = 9999; // 매매 마지막화면전호
        public const int REAL_SCREEN_NUM_START = 1000; // 실시간 시작화면번호
        public const int REAL_SCREEN_NUM_END = 1100; // 실시간 마지막화면번호
        public const int TR_SCREEN_NUM_START = 1100; // TR 초기화면번호
        public const int TR_SCREEN_NUM_END = 2000; // TR 마지막화면번호

        private int nTrScreenNum = TR_SCREEN_NUM_START; 
        private int nRealScreenNum = REAL_SCREEN_NUM_START;
        private int nTradeScreenNum = TRADE_SCREEN_NUM_START;

        // ------------------------------------------------------
        // 종목획득 변수
        // ------------------------------------------------------
        private string sConfigurationPath = @"D:\MJ\stock\getData\kiwoom\"; // 코스피, 코스닥 종목을 저장해놓은 파일의 디렉터리 경로
        private string[] kosdaqCodes; // 코스닥 종목들을 저장한 문자열 배열
        private string[] kospiCodes; //  코스피 종목들을 저장한 문자열 배열


        // ------------------------------------------------------
        // 공유 변수
        // ------------------------------------------------------
        public bool isMarketStart; // true면 장중, false면 장시작전,장마감후
        public string sAccountNum; // 계좌번호
        public int nSharedTime; // 모든 종목들이 공유하는 현재시간
        public int nCurDeposit;  // 현재 예수금
        public int nShutDown; // 장마감이 되면 양수가 됨.
        public bool isForCheckHoldings; // 현재잔고를 확인만 하기위한 기능
        public int nFirstDisposal; // 장시작이 되면 매도체크, only one chance: nFirstDisposal == 0

        // ------------------------------------------------------
        // 매매관련 변수
        // ------------------------------------------------------
        public int nMaxPriceForEachStock = 3000000;   // 각 종목이 최대 살 수 있는 금액 ex. 삼백만원
        public Queue<TradeSlot> tradeQueue = new Queue<TradeSlot>(); // 매매신청을 담는 큐, 매매컨트롤러가 사용할 큐
        TradeSlot curSlot; // 임시로 사용하능한 매매요청, 매매컨트롤러 변수
        public const int MAX_REQ_TIME = 1000;
        public const int MAX_REQ_SEC = 600;
        public bool[,] chanceFloorCntArray = new bool[2,31]; // 0은 코스닥, 1은 코스피 , 31설정은 -0 ~ -30까지를 의미
        //--------------------------------------------------------
        // 계좌평가잔고내역요청 변수
        //--------------------------------------------------------
        public Holdings[] holdingsArray = new Holdings[MAX_STOCK_HOLDINGS_NUM]; // 현재 보유주식을 담을 구조체 배열
        public int nHoldingCnt; // 총 보유주식의 수
        public int nCurHoldingsIdx; // 보유주식을 담을때 사용하는 인덱스 변수 

        //--------------------------------------------------------
        // 전고점 변수
        //--------------------------------------------------------
        public const int MAX_CRUSH_COUNT_X1_SEC = 60; // x1 의 최대
        public const int MAX_CRUSH_COUNT_X2_SEC = 120; // x2 의 최대
        public int[,] crushCounts = new int[MAX_CRUSH_COUNT_X1_SEC + 1, MAX_CRUSH_COUNT_X2_SEC + 1]; // + 1은 인덱스는 0부터니까

        //--------------------------------------------------------
        // 개인구조체 매수슬롯 변수
        //--------------------------------------------------------
        public const int BUY_SLOT_NUM = 50;
        public const int BUY_LIMIT_NUM = 5;
        public BuySlot[,] buySlotArray;
        public int[] buySlotCntArray;

        public string sBefore = "";//삭제예정
        public int nBeforeCnt;
        public int nBeforeCnt2;
        public int nBeforeShared;

        public Form1()
        {
            InitializeComponent(); // c# 고유 고정메소드  

            MappingFileToStockCodes(); // (0)


            // --------------------------------------------------
            // Winform Event Handler 
            // --------------------------------------------------
            checkMyAccountInfoButton.Click += Button_Click; 
            checkMyHoldingsButton.Click += Button_Click;
            
            // --------------------------------------------------
            // Event Handler 
            // --------------------------------------------------
            axKHOpenAPI1.OnEventConnect += OnEventConnectHandler; // 로그인 event slot connect
            axKHOpenAPI1.OnReceiveTrData += OnReceiveTrDataHandler; // TR event slot connect
            axKHOpenAPI1.OnReceiveRealData += OnReceiveRealDataHandler; // 실시간 event slot connect
            axKHOpenAPI1.OnReceiveChejanData += OnReceiveChejanDataHandler; // 체결,접수,잔고 event slot connect

            testTextBox.AppendText("로그인 시도..\r\n"); //++
            axKHOpenAPI1.CommConnect(); // 로그인 (1)
        }


        // ============================================
        // 버튼 클릭 이벤트의 핸들러 메소드
        // 1. 예수금상세현황요청
        // 2. 계좌평가잔고내역요청
        // 3. (테스트용) 강제 장시작 버튼
        // 4. (테스트용) 매도버튼
        // ============================================
        private void Button_Click(object sender, EventArgs e)
        {

            if (sender.Equals(checkMyAccountInfoButton)) // 예수금상세현황요청
            {
                RequestDeposit();
            }
            else if (sender.Equals(checkMyHoldingsButton)) // 계좌평가현황요청 
            {
                isForCheckHoldings = true;
                RequestHoldings(0);
            }
        }
        




        // ============================================
        // 주식종목들을 특정 txt파일에서 읽어
        // 코스닥, 코스피 변수에 string[] 형식으로 각각 저장
        // 코스닥, 코스피 종목갯수의 합만큼의 eachStockArray구조체 배열을 생성
        // ============================================
        private void MappingFileToStockCodes() 
        {
            kosdaqCodes = System.IO.File.ReadAllLines("today_kosdaq_stock_code.txt");
            kospiCodes = System.IO.File.ReadAllLines("today_kospi_stock_code.txt");
            
            //kosdaqCodes = new string[0];
            //kospiCodes = new string[0];

            testTextBox.AppendText("Kosdaq : " + kosdaqCodes.Length.ToString() + "\r\n");
            testTextBox.AppendText("Kospi : " + kospiCodes.Length.ToString() + "\r\n");

            eachStockArray = new EachStock[kosdaqCodes.Length + kospiCodes.Length];
            buySlotArray = new BuySlot[kosdaqCodes.Length + kospiCodes.Length, BUY_SLOT_NUM]; // ex. [1600, 50]
            buySlotCntArray = new int[kosdaqCodes.Length + kospiCodes.Length]; // ex. [1600]
        }




        // ============================================
        // 매매용 화면번호 재설정 메소드
        // ============================================
        private string SetTradeScreenNo()
        {
            if (nTradeScreenNum > TRADE_SCREEN_NUM_END)
                nTradeScreenNum = TRADE_SCREEN_NUM_START;

            string sTradeScreenNum = nTradeScreenNum.ToString();
            nTradeScreenNum++;
            return sTradeScreenNum;

        }

        // ============================================
        // 실시간용 화면번호 재설정 메소드
        // ============================================
        private string SetRealScreenNo()
        {
            if (nRealScreenNum > REAL_SCREEN_NUM_END)
                nRealScreenNum = REAL_SCREEN_NUM_START;

            string sRealScreenNum = nRealScreenNum.ToString();
            nRealScreenNum++;
            return sRealScreenNum;
        }


        // ============================================
        // Tr용 화면번호 재설정메소드
        // ============================================
        private string SetTrScreenNo()
        {
            if (nTrScreenNum > TR_SCREEN_NUM_END)
                nTrScreenNum = TR_SCREEN_NUM_START;

            string sTrScreenNum = nTrScreenNum.ToString();
            nTrScreenNum++;
            return sTrScreenNum;
        }




        // ============================================
        // string형  코스닥, 코스피 종목코드의 배열 string[n] 변수에서
        // 한 화면번호 당 (최대)100개씩 넣고 주식체결 fid를 넣고
        // 실시간 데이터 요청을 진행
        // 코스닥과 코스피 배열에서 100개가 안되는 나머지 종목들은 코스닥,코스피 각 다른 화면번호에 실시간 데이터 요청
        // ============================================
        private void SubscribeRealData()
        {
            testTextBox.AppendText("구독 시작..\r\n"); //++
            int kosdaqIndex = 0;
            int kosdaqCodesLength = kosdaqCodes.Length;
            int kosdaqIterNum = kosdaqCodesLength / NUM_SEP_PER_SCREEN;
            int kosdaqRestNum = kosdaqCodesLength % NUM_SEP_PER_SCREEN;
            string strKosdaqCodeList;
            const string sFID ="228" ; // 체결강도. 실시간 목록 FID들 중 겹치는게 가장 적은 FID
            string sScreenNum;
            // ------------------------------------------------------
            // 코스닥 실시간 등록
            // ------------------------------------------------------
            // 100개 단위
            for (int i = 0; i< kosdaqIterNum; i++)
            {
                sScreenNum = SetRealScreenNo();
                strKosdaqCodeList = ConvertStrCodeList(kosdaqCodes, kosdaqIndex, kosdaqIndex + NUM_SEP_PER_SCREEN, KOSDAQ_ID, sScreenNum);
                axKHOpenAPI1.SetRealReg(sScreenNum, strKosdaqCodeList, sFID, "0");
                kosdaqIndex += NUM_SEP_PER_SCREEN;
            }
            // 나머지
            if (kosdaqRestNum > 0)
            {
                sScreenNum = SetRealScreenNo();
                strKosdaqCodeList = ConvertStrCodeList(kosdaqCodes, kosdaqIndex, kosdaqIndex + kosdaqRestNum, KOSDAQ_ID, sScreenNum);
                axKHOpenAPI1.SetRealReg(sScreenNum, strKosdaqCodeList, sFID, "0");
            }

            int kospiIndex = 0;
            int kospiCodesLength = kospiCodes.Length;
            int kospiIterNum = kospiCodesLength / NUM_SEP_PER_SCREEN;
            int kospiRestNum = kospiCodesLength % NUM_SEP_PER_SCREEN;
            string strKospiCodeList;

            // ------------------------------------------------------
            // 코스피 실시간 등록
            // ------------------------------------------------------
            // 100개 단위
            for (int i = 0; i < kospiIterNum; i++)
            {
                sScreenNum = SetRealScreenNo();
                strKospiCodeList = ConvertStrCodeList(kospiCodes, kospiIndex, kospiIndex + NUM_SEP_PER_SCREEN, KOSPI_ID, sScreenNum);
                axKHOpenAPI1.SetRealReg(sScreenNum, strKospiCodeList, sFID, "0");
                kospiIndex += NUM_SEP_PER_SCREEN;
            }
            // 나머지
            if (kospiRestNum > 0)
            {
                sScreenNum = SetRealScreenNo();
                strKospiCodeList = ConvertStrCodeList(kospiCodes, kospiIndex, kospiIndex + kospiRestNum, KOSPI_ID, sScreenNum);
                axKHOpenAPI1.SetRealReg(sScreenNum, strKospiCodeList, sFID, "0");
            }
            testTextBox.AppendText("구독 완료..\r\n"); //++
        }




        // ============================================
        // 매개변수 : 
        //  1.  string[] codes : 주식종목코드 배열
        //  2.  s : 배열의 시작 인덱스
        //  3.  e : 배열의 끝 인덱스 (포함 x)
        //  4.  marketGubun : 코스닥, 코스피 구별변수
        //  5.  sScreenNum : 실시간 화면번호
        //
        // 키움 실시간 신청메소드의 두번째 인자인 strCodeList는
        // 종목코드1;종목코드2;종목코드3;....;종목코드n(;마지막은 생략가능) 형식으로 넘겨줘야하기 때문에
        // s부터 e -1 인덱스까지 string 변수에 추가하며 사이사이 ';'을 붙여준다
        //
        // 실시간메소드에서 각 종목의 구조체를 사용하기 위해 초기화과정이 필요한데
        // 이 메소드에서 같이 진행해준다.
        // ============================================
        private string ConvertStrCodeList(string[] codes, int s, int e, int marketGubun, string sScreenNum)
        {
            string strKosdaqCodeList = "";
            for (int i = s; i < e; i++)
            {
                int codeIdx = int.Parse(codes[i]);

                // TODO. Map(java) 기능과 속도 비교 후 수정 예정
                ////// eachStockIdx 설정 부분 ///////
                eachStockIdxArray[codeIdx] = nEachStockIdx;
                nEachStockIdx++;
                /////////////////////////////////////

                ////// eachStock 초기화 부분 //////////
                nCurIdx = eachStockIdxArray[codeIdx];
                eachStockArray[nCurIdx].sRealScreenNum = sScreenNum;
                eachStockArray[nCurIdx].sCode = codes[i];
                eachStockArray[nCurIdx].fMaxPower = -100.0;
                eachStockArray[nCurIdx].nMarketGubun = marketGubun;
                //////////////////////////////////////

                strKosdaqCodeList += codes[i];
                if (i < e - 1)
                    strKosdaqCodeList += ';';
            }
            return strKosdaqCodeList;
        }




        // ============================================
        // 로그인 이벤트발생시 핸들러 메소드
        // ============================================
        private void OnEventConnectHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0) // 로그인 성공
            {
                testTextBox.AppendText("로그인 성공\r\n"); //++
                string sMyName = axKHOpenAPI1.GetLoginInfo("USER_NAME");
                string sAccList = axKHOpenAPI1.GetLoginInfo("ACCLIST"); // 로그인 사용자 계좌번호 리스트 요청
                string[] accountArray = sAccList.Split(';');

                sAccountNum = accountArray[0]; // 처음계좌가 main계좌
                accountComboBox.Text = sAccountNum;
                SubscribeRealData(); // 실시간 구독 
                RequestDeposit(); // 예수금상세현황요청 
                isMarketStart = true;//오늘삭제
                foreach (string sAccount in accountArray)
                {
                    if (sAccount.Length > 0)
                        accountComboBox.Items.Add(sAccount);
                }
                myNameLabel.Text = sMyName;
                
            }
            else
            {
                MessageBox.Show("로그인 실패");
            }
        } // END ---- 로그인 이벤트 핸들러





        // ============================================
        // 계좌평가잔고내역요청 TR요청메소드
        // CommRqData 3번째 인자 sPrevNext가 0일 경우 처음 20개의 종목을 요청하고
        // 2일 경우 초기20개 초과되는 종목들을 계속해서 요청한다.
        // ============================================
        private void RequestHoldings(int sPrevNext)
        {
            if (sPrevNext == 0)
            {
                nHoldingCnt = 0;
                nCurHoldingsIdx = 0;
            }
            SetTrScreenNo();
            axKHOpenAPI1.SetInputValue("계좌번호", sAccountNum);
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
            axKHOpenAPI1.SetInputValue("조회구분", "2"); // 1:합산 2:개별
            axKHOpenAPI1.CommRqData("계좌평가잔고내역요청", "opw00018", sPrevNext, SetTrScreenNo());
        }


        // ============================================
        // 예수금상세현황요청 TR요청메소드
        // ============================================
        private void RequestDeposit()
        {
            axKHOpenAPI1.SetInputValue("계좌번호", sAccountNum);
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
            axKHOpenAPI1.SetInputValue("조회구분", "2");
            axKHOpenAPI1.CommRqData("예수금상세현황요청", "opw00001", 0, SetTrScreenNo());
        }

        // ============================================
        // 당일실현손익상세요청 TR요청메소드
        // ============================================
        private void RequestTradeResult()
        {
            axKHOpenAPI1.SetInputValue("계좌번호", sAccountNum);
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("종목코드", "");
            axKHOpenAPI1.CommRqData("당일실현손익상세요청", "opt10077", 0, SetTrScreenNo());
        }

        // ============================================
        // TR 이벤트발생시 핸들러 메소드
        // ============================================
        private void OnReceiveTrDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sRQName.Equals("예수금상세현황요청"))
            {
                nCurDeposit = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, 0, "주문가능금액")));
                isDepositSet = true;
                testTextBox.AppendText("예수금 세팅 완료\r\n"); //++
                myDepositLabel.Text = nCurDeposit.ToString() + "(원)";
            }
            else if (e.sRQName.Equals("계좌평가잔고내역요청"))
            {
                int rows = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRecordName);
                nHoldingCnt += rows;

                for (int i = 0; nCurHoldingsIdx < nHoldingCnt; nCurHoldingsIdx++, i++)
                {
                    holdingsArray[nCurHoldingsIdx].sCode = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "종목번호").Trim().Substring(1);
                    holdingsArray[nCurHoldingsIdx].sCodeName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "종목명").Trim();
                    holdingsArray[nCurHoldingsIdx].fYield = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "수익률(%)"));
                    holdingsArray[nCurHoldingsIdx].nHoldingQty = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "보유수량")));
                    holdingsArray[nCurHoldingsIdx].nBuyedPrice = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "매입가")));
                    holdingsArray[nCurHoldingsIdx].nCurPrice = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "현재가")));
                    holdingsArray[nCurHoldingsIdx].nTotalPL = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "평가손익")));
                    holdingsArray[nCurHoldingsIdx].nNumPossibleToSell = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "매매가능수량")));
                }

                if (e.sPrevNext.Equals("2"))
                {
                    RequestHoldings(2);
                }
                else // 보유잔고 확인 끝
                {
                    if (isForCheckHoldings)
                    {
                        isForCheckHoldings = false;

                        for (int i = 0; i < nHoldingCnt; i++)
                        {
                            testTextBox.AppendText("종목번호 : "+ holdingsArray[i].sCode+", 종목명 : "+ holdingsArray[i].sCodeName +", 보유수량 : "+ holdingsArray[i].nHoldingQty + ", 평가손익 : " + holdingsArray[i].nTotalPL + "\r\n");
                        }
                    }
                    else if( (nFirstDisposal==0) || (nShutDown > 0))
                    {
                        nFirstDisposal++;
                        for (int i = 0; i < nHoldingCnt; i++)
                        {
                            axKHOpenAPI1.SendOrder("시간초과매도", SetTradeScreenNo(), sAccountNum, 2, holdingsArray[i].sCode, holdingsArray[i].nNumPossibleToSell, 0, "03", "");
                            Thread.Sleep(200); // 1초에 5번 제한
                        }
                    }
                }
            } // END ---- e.sRQName.Equals("계좌평가잔고내역요청")
            else if(e.sRQName.Equals("당일실현손익상세요청"))
            {
                int nTodayResult = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, 0, "당일실현손익"));
                testTextBox.AppendText("당일실현손익 : " + nTodayResult + "(원) \r\n"); //++
                int rows = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRecordName);
               
                string sCode;
                string sCodeName;
                int nTradeVolume;
                double fBuyPrice;
                int nTradePrice;
                int nTodayPL;
                double fYield;

                for (int i = 0; i < rows; i++)
                {
                    sCode = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "종목코드").Trim().Substring(1);
                    sCodeName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "종목명").Trim();
                    fYield = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "손익율"));
                    nTradeVolume = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "체결량")));
                    fBuyPrice = Math.Abs(double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "매입단가")));
                    nTradePrice = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "체결가")));
                    nTodayPL = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "당일매도손익"));

                    testTextBox.AppendText("종목명 : "+ sCodeName + ", 종목코드 : " + sCode+", 체결량 : "+ nTradeVolume +", 매입단가 : "+fBuyPrice+", 체결가 : "+nTradePrice+", 손익율 : "+fYield+"(%), 당일매도손익 : "+nTodayPL + "\r\n");
                }
            } // END ---- e.sRQName.Equals("당일실현손익상세요청")
        } // END ---- TR 이벤트 핸들러




      
        // ============================================
        // 실시간 이벤트발생시 핸들러메소드 
        // ============================================
        private void OnReceiveRealDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {

            if (e.sRealType.Equals("주식체결"))
            {
                string sCode = e.sRealKey;
                eachStockArray[nCurIdx].nTime = nSharedTime = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(sCode, 20))); // 현재시간
                string sCurTime = DateTime.Now.ToString("hhmmss");//삭제
                nBeforeCnt++;
                nBeforeCnt2++;
                if (!sBefore.Equals(sCurTime))
                {
                    sBefore = sCurTime;
                    testTextBox.AppendText("---" +nBeforeCnt.ToString() + "\r\n");
                    nBeforeCnt = 0;
                }

                ////testTextBox.AppendText(SubTimeToTimeAndSec(int.Parse(sCurTime), nSharedTime).ToString() + " : " + sCurTime + " ," + nSharedTime.ToString() + "\r\n");//삭제

                if (nBeforeShared != nSharedTime)
                {
                    testTextBox.AppendText("#########"+  nBeforeCnt2.ToString() + "\r\n");
                    nBeforeShared = nSharedTime;
                    nBeforeCnt2 = 0;

                }













                // -------------------------------------------------------------------------
                // 매매컨트롤러 부분
                // 매매컨트롤러 부분은 Async를 사용하려 했는데
                // 오버헤드가 클거같아서 실시간 부분에 끼워넣었다.
                // -------------------------------------------------------------------------
                // 예수금이 세팅돼있고 큐에 하나이상의 슬롯이 존재하면 접근 가능


                if (isDepositSet && (tradeQueue.Count > 0)) // START ---- 매매컨트롤러
                {
                    curSlot = tradeQueue.Dequeue(); // 우선 디큐한다

                    if (SubTimeToTimeAndSec(nSharedTime, curSlot.nRqTime) <= IGNORE_REQ_SEC) // 현재시간 - 요청시간 < 10초 : 요청시간이 너무 길어진 요청의 처리를 위한 분기문  
                    {

                        if (curSlot.nOrderType <= 2) // 신규매수매도 신규매수:1 신규매도:2
                        {
                            // 아직 매수중이거나 매도중일때는
                            if ((eachStockArray[curSlot.nEachStockIdx].nBuyReqCnt > 0) || (eachStockArray[curSlot.nEachStockIdx].nSellReqCnt > 0)) // 현재 거래중이면
                            {
                                if (curSlot.nOrderType == 2) // 매도신청은 버려져서는 안됨.
                                    curSlot.nRqTime = nSharedTime; // 요청시간을 현재시간으로 설정
                                tradeQueue.Enqueue(curSlot); // 디큐했던 슬롯을 다시 인큐한다.
                            }
                            else // 거래중이 아닐때 (단, 매수취소는 예외)
                            {
                                if (curSlot.nOrderType == 1) // 신규매수
                                {
                                    int nEstimatedPrice = curSlot.nOrderPrice; // 종목의 요청했던 최우선매도호가를 받아온다.
                                    // 반복해서 가격을 n칸 올린다.
                                    if (eachStockArray[curSlot.nEachStockIdx].nMarketGubun == 1) // 코스닥일 경우
                                    {
                                        for (int i = 0; i < EYES_CLOSE_NUM; i++)
                                            nEstimatedPrice += GetKosdaqGap(nEstimatedPrice);
                                    }
                                    else if (eachStockArray[curSlot.nEachStockIdx].nMarketGubun == 2) // 코스피의 경우
                                    {
                                        for (int i = 0; i < EYES_CLOSE_NUM; i++)
                                            nEstimatedPrice += GetKospiGap(nEstimatedPrice);
                                    }

                                    double fCurLimitPriceFee = (nEstimatedPrice * (1 + VIRTUAL_STOCK_FEE));

                                    int nNumToBuy = (int)(nCurDeposit / fCurLimitPriceFee); // 현재 예수금으로 살 수 있을 만큼
                                    int nMaxNumToBuy = (int)(nMaxPriceForEachStock / fCurLimitPriceFee); // 최대매수가능금액으로 살 수 있을 만큼

                                    if (nNumToBuy > nMaxNumToBuy) // 최대매수가능수를 넘는다면
                                        nNumToBuy = nMaxNumToBuy; // 최대매수가능수로 세팅

                                    // 구매수량이 있고 현재종목의 최우선매도호가가 요청하려는 지정가보다 낮을 경우 구매요청을 걸 수 있다.
                                    if ((nNumToBuy > 0) && (eachStockArray[curSlot.nEachStockIdx].nFs < nEstimatedPrice))
                                    {
                                        if (curSlot.sHogaGb.Equals("03")) // 시장가모드 : 시장가로 하면 키움에서 상한가값으로 계산해서 예수금만큼 살 수 가 없다
                                        {
                                            if (buySlotCntArray[curSlot.nEachStockIdx] < BUY_LIMIT_NUM) // 개인 구매횟수를 넘기지 않았다면
                                            {
                                                eachStockArray[curSlot.nEachStockIdx].nCurLimitPrice = nEstimatedPrice; // 지정상한가 설정
                                                eachStockArray[curSlot.nEachStockIdx].fTargetPercent = curSlot.fTargetPercent; // 익절퍼센트 설정
                                                eachStockArray[curSlot.nEachStockIdx].fBottomPercent = curSlot.fBottomPercent; // 손절퍼센트 설정
                                                eachStockArray[curSlot.nEachStockIdx].nCurBuySlotIdx = buySlotCntArray[curSlot.nEachStockIdx]; // 종목의 현재 거래레코드 인덱스(종목에 한번도 매수를 안했었으면 0)
                                                eachStockArray[curSlot.nEachStockIdx].nCurRqTime = nSharedTime; // 현재시간설정

                                                testTextBox.AppendText(eachStockArray[curSlot.nEachStockIdx].nCurRqTime + " : " + sCode + " 매수신청 전송 \r\n"); //++
                                                int nBuyReqResult = axKHOpenAPI1.SendOrder(curSlot.sRQName, SetTradeScreenNo(), sAccountNum,
                                                    curSlot.nOrderType, curSlot.sCode, nNumToBuy, nEstimatedPrice,
                                                    "00", curSlot.sOrgOrderId); // 높은 매도호가에 지정가로 걸어 시장가처럼 사게 한다
                                                                                // 최우선매도호가보다 높은 가격에 지정가를 걸면 현재매도호가에 구매하게 된다.
                                                if (nBuyReqResult == 0) // 요청이 성공하면
                                                {
                                                    eachStockArray[curSlot.nEachStockIdx].nBuyReqCnt++; // 구매횟수 증가
                                                    testTextBox.AppendText(sCode + " 매수신청 전송 성공 \r\n"); //++
                                                }
                                            }
                                            else  // 개인 구매횟수를 넘겼다면
                                                testTextBox.AppendText(curSlot.sCode + " 종목의 구매횟수를 초과했습니다.\r\n");
                                        }
                                    }
                                } // END ---- 신규매수
                                else if (curSlot.nOrderType == 2) // 신규매도
                                {
                                    if (curSlot.sHogaGb.Equals("03")) // 시장가매도
                                    {

                                        eachStockArray[curSlot.nEachStockIdx].nCurBuySlotIdx = curSlot.nBuySlotIdx; // 필요없는 작업인데 혹시 몰라 남겨놓음
                                        eachStockArray[curSlot.nEachStockIdx].nCurRqTime = nSharedTime; // 현재시간설정

                                        testTextBox.AppendText(nSharedTime + " : " + sCode + " 매도신청 전송 \r\n"); //++
                                        int nSellReqResult = axKHOpenAPI1.SendOrder(curSlot.sRQName, SetTradeScreenNo(), sAccountNum,
                                                curSlot.nOrderType, curSlot.sCode, curSlot.nQty, 0,
                                                curSlot.sHogaGb, curSlot.sOrgOrderId);

                                        if (nSellReqResult != 0) // 요청이 성공하지 않으면
                                        {
                                            testTextBox.AppendText(sCode + " 매도신청 전송 실패 \r\n"); //++
                                            buySlotArray[curSlot.nEachStockIdx, curSlot.nBuySlotIdx].isSelled = false; // 요청실패일때 다시 요청하기 위해
                                            // 해당 buySlot에서 판매완료시그널을 false로 세팅해준다
                                            // 이 작업을 하기 위해서 TradeSlot 구조체에 nBuySlotIdx 변수가 필요한것이다.
                                        }
                                        else
                                        {
                                            testTextBox.AppendText(sCode + " 매도신청 전송 성공 \r\n"); //++
                                            eachStockArray[curSlot.nEachStockIdx].nSellReqCnt++; // 매도요청전송이 성공하면 매도횟수를 증가한다.
                                        }
                                    }
                                } // END ---- 신규매도
                            }
                        } // End ---- 신규매수매도
                        else if (curSlot.nOrderType == 3) // 매수취소 매수취소는 매수중일때만 요청되고 매수와 함께 슬롯입장이 가능하다. 매도중일때는 안된다.
                        {
                            // 구매중일때만 매수취소가 가능하니 buySlotArray의 인덱스는 매수취소종목의 마지막인덱스로 확정되니
                            // 건들지 않는다

                            testTextBox.AppendText(nSharedTime + " : " + sCode + " 매수취소신청 전송 \r\n"); //++
                            int nCancelReqResult = axKHOpenAPI1.SendOrder(curSlot.sRQName, SetTradeScreenNo(), sAccountNum,
                                curSlot.nOrderType, curSlot.sCode, 0, 0,
                                "", curSlot.sOrgOrderId);

                            if (nCancelReqResult != 0) // 매수취소가 성공하지 않으면
                            {
                                eachStockArray[curSlot.nEachStockIdx].isCancelMode = false; // 해당종목의 현재 매수취소시그널을 false한다
                                // 이래야지 매수취소를 다시 신청할 수 있다.
                            }
                            else
                            {
                                testTextBox.AppendText(sCode + " 매수취소신청 전송 성공 \r\n"); //++
                            }
                        } // End ---- 매수취소
                    } // End ---- 현재시간 - 요청시간 < 10초

                } // END ---- 매매컨트롤러








                // -------------------------------------------------------------------------
                // 실시간 데이터 처리
                // 여기서부터 아래는 전략을 담는 구간이다
                // 이 윗 구간은 모든 실시간 주식체결을 거쳐가는 매매컨트롤러 부분이었다
                // -------------------------------------------------------------------------


                if (isMarketStart) // 장이 시작 안했으면 접근 금지
                {
                    if (nSharedTime >= SHUTDOWN_TIME) // 3시가 넘었으면
                    {
                        testTextBox.AppendText("3시가 지났다\r\n");
                        nShutDown++; // 장이 끝남을 알린다 (nShutDown이 0일때는 전량매도작업을 수행하지 않기 때문에)
                        isMarketStart = false; // 장 중 시그널을 off한다
                        for (int nScreenNum = REAL_SCREEN_NUM_START; nScreenNum <= REAL_SCREEN_NUM_END; nScreenNum++)
                        {
                            // 실시간 체결에 할당된 화면번호들에 대해 다 디스커넥트한다
                            // 실시간 체결만 받고있는 화면번호들만이 아니라 전부를 디스커넥트하는 이유는
                            // 전부를 디스커넥트하는 잠깐의 시간동안 잔여 실시간주식체결데이터들이 처리되는 것을 기다리는 기능도 있다.
                            axKHOpenAPI1.DisconnectRealData(nScreenNum.ToString());
                        }
                        RequestHoldings(0); // 잔고현황을 체크한다. 이때 nShutDown이 양수이기 때문에 남아있는 주식들이 있으면 전량 매도한다.
                        return;
                    }


                    int nCodeIdx = int.Parse(sCode);
                    nCurIdx = eachStockIdxArray[nCodeIdx];

                    eachStockArray[nCurIdx].nFs = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(sCode, 27))); // 최우선매도호가
                    eachStockArray[nCurIdx].nFb = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(sCode, 28))); // 최우선매수호가
                    eachStockArray[nCurIdx].nTv = int.Parse(axKHOpenAPI1.GetCommRealData(sCode, 15)); // 현재 체결량
                    eachStockArray[nCurIdx].nIdx++; // 인덱스를 올린다.


                    // 현재 거래중이고(nBuyReqCnt > 0) 현재 매수취소가 가능한 상태라면 접근 가능

                    if (eachStockArray[nCurIdx].isOrderStatus)
                    {
                        if ((eachStockArray[nCurIdx].nBuyReqCnt > 0) && !eachStockArray[nCurIdx].isCancelMode) // 미체결량이 남아있다면
                        {
                            // 현재 최우선매도호가가 지정상한가를 넘었거나 매매 요청시간과 현재시간이 너무 오래 차이난다면(= 매수가 너무 오래걸린다 = 거래량이 낮고 머 별거 없다)
                            if ((eachStockArray[nCurIdx].nFs > eachStockArray[nCurIdx].nCurLimitPrice) || (SubTimeToTimeAndSec(nSharedTime, eachStockArray[nCurIdx].nCurRqTime) >= MAX_REQ_SEC)) // 지정가를 초과하거나 오래걸린다면
                            {
                                curSlot.sRQName = "매수취소";
                                curSlot.nOrderType = 3; // 매수취소
                                curSlot.sCode = sCode;
                                curSlot.sOrgOrderId = eachStockArray[nCurIdx].sCurOrgBuyId; // 현재 매수의 원주문번호를 넣어준다.
                                curSlot.nRqTime = eachStockArray[nCurIdx].nTime; // 현재시간설정

                                tradeQueue.Enqueue(curSlot); // 매매신청큐에 인큐

                                eachStockArray[nCurIdx].isCancelMode = true; // 현재 매수취소 불가능상태로 만든다
                                testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 매수취소신청 \r\n"); //++
                            }
                        }
                    }



                    // 이걸 어찌해야하나 고민중이다... nHoldingCnt말고 더 좋은 변수를 이자리에 넣을 수 있을거같은 생각이 든다.
                    if (eachStockArray[nCurIdx].nHoldingsCnt > 0) // 보유종목이 있다면
                    {
                        int nBuySlotIdx = buySlotCntArray[nCurIdx]; // 현재 종목의 매수 record수를 체크
                                                                    // 한번의 레코드 신청이 있다하더라도 매수가 완료된 시점에 ++하기 떄문에
                                                                    // 처음에는 0일테고 하나의 거래가 완료되면 1이 되니 그때부터 for문에서 접근 가능
                        int nBuyPrice;
                        double fYield;

                        for (int i = 0; i < nBuySlotIdx; i++) // 반복적 확인
                        {
                            // 그리고 !isSelled 아직 판매완료가 안됐을때 접근 가능
                            if (!buySlotArray[nCurIdx, i].isSelled)
                            {
                                bool isSell = false;

                                nBuyPrice = buySlotArray[nCurIdx, i].nBuyPrice; // 처음 초기화됐을때는 0인데 체결이 된 상태에서만 접근 가능하니 사졌을 때의 평균매입가
                                fYield = (eachStockArray[nCurIdx].nFb - nBuyPrice) / nBuyPrice; // 현재 최우선매수호가 와의 손익률을 구한다
                                if (fYield >= buySlotArray[nCurIdx, i].fTargetPer + (STOCK_TAX + STOCK_FEE + STOCK_FEE)) // 손익률이 익절퍼센트를 넘기면
                                {
                                    isSell = true;
                                    curSlot.sRQName = "익절매도";
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 익절매도신청 \r\n"); //++
                                }
                                else if (fYield <= buySlotArray[nCurIdx, i].fBottomPer + (STOCK_TAX + STOCK_FEE + STOCK_FEE)) // 손익률이 손절퍼센트보다 낮으면
                                {
                                    isSell = true;
                                    curSlot.sRQName = "손절매도";
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 손절매도신청 \r\n"); //++
                                }

                                if (isSell)
                                {
                                    curSlot.nOrderType = 2; // 신규매도
                                    curSlot.sCode = sCode;
                                    curSlot.nQty = buySlotArray[nCurIdx, i].nBuyVolume; // 이 레코드에 있는 전량을 판매한다
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nBuySlotIdx = i; // 나중에 요청전송이 실패할때 다시 취소하기 위해 적어놓는 변수
                                    curSlot.nEachStockIdx = nCurIdx; // 현재 종목의 인덱스
                                    curSlot.nRqTime = eachStockArray[nCurIdx].nTime; // 현재시간 설정

                                    tradeQueue.Enqueue(curSlot); // 매매요청큐에 인큐한다
                                    buySlotArray[nCurIdx, i].isSelled = true; // 현재 거래레코드는 판매완료됐다, 요청전송 실패됐을때는 다시 false로 설정된다.

                                }
                            }
                        } // END ---- 반복적 확인 종료
                    } // END ---- 보유종목이 있다면




                    // 처음가격과 시간등을 맞추려는 변수이다.
                    if (!eachStockArray[nCurIdx].isFirstCheck) // 개인 초기작업
                    {
                        if (eachStockArray[nCurIdx].nFirstTime == 0) // 처음일때 시간설정
                        {
                            eachStockArray[nCurIdx].nFirstTime = eachStockArray[nCurIdx].nTime; // 장시작되고 처음 얻어지는 시간
                            eachStockArray[nCurIdx].nTenTime = AddTimeBySec(eachStockArray[nCurIdx].nFirstTime, 3600); // 처음시간에 + 3600초(1시간)을 더한 시간
                            eachStockArray[nCurIdx].nNoonTime = 120000; // 12시
                        }

                        if (eachStockArray[nCurIdx].nFs == 0 && eachStockArray[nCurIdx].nFb == 0)  // 둘 다 데이터가 없는경우는 가격초기화가 불가능하기 return
                            return;
                        else
                        {
                            // 둘다 제대로 받아졌거나 , 둘 중 하나가 안받아졌거나
                            if (eachStockArray[nCurIdx].nFs == 0) // fs가 안받아졌으면 fb 가격에 fb갭 한칸을 더해서 설정
                            {
                                int gap = 0;
                                if (eachStockArray[nCurIdx].nMarketGubun == KOSDAQ_ID)
                                    gap = GetKosdaqGap(eachStockArray[nCurIdx].nFb);
                                else if (eachStockArray[nCurIdx].nMarketGubun == KOSPI_ID)
                                    gap = GetKospiGap(eachStockArray[nCurIdx].nFb);

                                eachStockArray[nCurIdx].nFs = eachStockArray[nCurIdx].nFb + gap;
                            }


                            if (eachStockArray[nCurIdx].nFb == 0) // fb가 안받아졌으면 fs 가격에 (fs-1)갭 한칸을 마이너스해서 설정
                            {
                                // fs-1 인 이유는 fs가 1000원이라하면 fb는 999여야하는데 갭을 받을때 5를 받게되니 fb가 995가 되어버린다.이는 오류!
                                int gap = 0;
                                if (eachStockArray[nCurIdx].nMarketGubun == KOSDAQ_ID)
                                    gap = GetKosdaqGap(eachStockArray[nCurIdx].nFs - 1);
                                else if (eachStockArray[nCurIdx].nMarketGubun == KOSPI_ID)
                                    gap = GetKospiGap(eachStockArray[nCurIdx].nFs - 1);

                                eachStockArray[nCurIdx].nFb = eachStockArray[nCurIdx].nFs - gap;
                            }

                            eachStockArray[nCurIdx].nFirstPrice = eachStockArray[nCurIdx].nFs;

                            if (eachStockArray[nCurIdx].nFs < 1000) // 1000원도 안한다면 폐기처분
                            {
                                axKHOpenAPI1.SetRealRemove(eachStockArray[nCurIdx].sRealScreenNum, eachStockArray[nCurIdx].sCode);
                            }
                            eachStockArray[nCurIdx].isFirstCheck = true; // 가격설정이 끝났으면 이종목의 초기체크는 완료 설정
                        }
                    } // END ---- 개인 초기작업


                    // 파워는 최우선매수호가와 초기가격의 손익률로 계산한다
                    eachStockArray[nCurIdx].fPower = (double)(eachStockArray[nCurIdx].nFb - eachStockArray[nCurIdx].nFirstPrice) / eachStockArray[nCurIdx].nFirstPrice;

                    // 이상 데이터 감지
                    // fs와 fb의 가격차이가 2퍼가 넘을경우 이상데이터라 생각하고 리턴한다.
                    // 미리 리턴하는 이유는 이런 이상 데이터로는 전략에 사용하지 않기위해서 전략찾는 부분 위에서 리턴여부를 검증한다.
                    if ((eachStockArray[nCurIdx].nFs - eachStockArray[nCurIdx].nFb) / eachStockArray[nCurIdx].nFb > 0.02)
                        return;

                    // -----------------------------------------------
                    // 코스피, 코스닥 공용 전략 
                    // 이 구간에는 코스피와 코스닥에 공동으로 사용가능한 전략들을
                    // 찾는 구간이다
                    // -----------------------------------------------

                    // END ---- 코스피, 코스닥 공용 전략 


                    if (eachStockArray[nCurIdx].nMarketGubun == KOSDAQ_ID) // 코스닥 한정 전략
                    {

                        // -----------------------------------------
                        // 전고점 돌파 처리
                        // -----------------------------------------
                        if ((eachStockArray[nCurIdx].fPower > eachStockArray[nCurIdx].fMaxPower) &&
                            ((eachStockArray[nCurIdx].fMaxPower - eachStockArray[nCurIdx].fMinPower) > 0.01) &&
                            (eachStockArray[nCurIdx].nTime < BAN_BUY_TIME))
                        {


                            if ((crushCounts[10, 10] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 10) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 10))
                            {
                                crushCounts[10, 10]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    // 만약 조건이 만족하여 매수하게 된다면
                                    // 중요하게 필요한것은 
                                    // 현재 인덱스와 현재 가격
                                    // 익절퍼센트, 손절퍼센트가 유의미하게 필요하다
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.04;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 10,10(1) 매수신청 \r\n"); //++
                                }
                            }

                            if ((crushCounts[10, 10] == 1) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 10) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 10))
                            {
                                crushCounts[10, 10]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.03;
                                    curSlot.fBottomPercent = -0.07;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 10,10(2) 매수신청 \r\n"); //++
                                }
                            }

                            if ((crushCounts[15, 15] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 15) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 15))
                            {
                                crushCounts[15, 15]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 15,15 매수신청 \r\n"); //++
                                }
                            }

                            if ((crushCounts[20, 20] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 20) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 20))
                            {
                                crushCounts[20, 20]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 20,20 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[40, 40] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 40) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 40))
                            {
                                crushCounts[40, 40]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 40,40 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[10, 30] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 10) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 30))
                            {
                                crushCounts[10, 30]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 10,30 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[10, 60] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 10) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 60))
                            {
                                crushCounts[10, 60]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 10,60 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[30, 60] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 30) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 60))
                            {
                                crushCounts[30, 60]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 30,60 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[30, 30] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 30) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 30))
                            {
                                crushCounts[30, 30]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 30,30 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[5, 30] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 5) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 30))
                            {
                                crushCounts[5, 30]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 5,30 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[5, 60] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 5) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 60))
                            {
                                crushCounts[5, 60]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.06;
                                    curSlot.fBottomPercent = -0.05;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 5,60 매수신청 \r\n"); //++
                                }
                            }

                            if ((crushCounts[1, 5] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 1) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 5))
                            {
                                crushCounts[1, 5]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.04;
                                    curSlot.fBottomPercent = -0.03;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 1,5 매수신청 \r\n"); //++
                                }
                            }
                            if ((crushCounts[1, 1] == 0) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nMinTime, eachStockArray[nCurIdx].nMaxTime) >= 1) &&
                                 (SubTimeToTimeAndSec(eachStockArray[nCurIdx].nTime, eachStockArray[nCurIdx].nMaxTime) >= 1))
                            {
                                crushCounts[1, 1]++;
                                if (eachStockArray[nCurIdx].fPower >= 0.1 && eachStockArray[nCurIdx].fPower <= 0.15 && eachStockArray[nCurIdx].nIdx > 100)
                                {
                                    curSlot.sRQName = "전고점돌파";
                                    curSlot.nOrderType = 1; // 신규매수
                                    curSlot.sCode = sCode;
                                    curSlot.nEachStockIdx = nCurIdx;
                                    curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                    curSlot.sHogaGb = "03";
                                    curSlot.sOrgOrderId = "";
                                    curSlot.nRqTime = nSharedTime;
                                    curSlot.fTargetPercent = 0.04;
                                    curSlot.fBottomPercent = -0.03;

                                    tradeQueue.Enqueue(curSlot);
                                    testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 1,1 매수신청 \r\n"); //++
                                }
                            }
                        }

                        if (eachStockArray[nCurIdx].fMaxPower < eachStockArray[nCurIdx].fPower)
                        {
                            eachStockArray[nCurIdx].fMaxPower = eachStockArray[nCurIdx].fPower;
                            eachStockArray[nCurIdx].nMaxTime = eachStockArray[nCurIdx].nTime;
                            eachStockArray[nCurIdx].fMinPower = eachStockArray[nCurIdx].fPower;
                            eachStockArray[nCurIdx].nMinTime = eachStockArray[nCurIdx].nTime;
                        }

                        if (eachStockArray[nCurIdx].fMinPower > eachStockArray[nCurIdx].fPower)
                        {
                            eachStockArray[nCurIdx].fMinPower = eachStockArray[nCurIdx].fPower;
                            eachStockArray[nCurIdx].nMinTime = eachStockArray[nCurIdx].nTime;
                        }// END ---- 전고점돌파


                        // -----------------------------------------
                        // 바닥잡기 처리
                        // -----------------------------------------
                        if (!chanceFloorCntArray[KOSDAQ_ID, 17] &&
                            (eachStockArray[nCurIdx].nTime < eachStockArray[nCurIdx].nNoonTime) &&
                             (eachStockArray[nCurIdx].fPower < -0.17))
                        {
                            if (eachStockArray[nCurIdx].nIdx > 200)
                            {
                                curSlot.sRQName = "바닥잡기";
                                curSlot.nOrderType = 1; // 신규매수
                                curSlot.sCode = sCode;
                                curSlot.nEachStockIdx = nCurIdx;
                                curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                curSlot.sHogaGb = "03";
                                curSlot.sOrgOrderId = "";
                                curSlot.nRqTime = nSharedTime;
                                curSlot.fTargetPercent = 0.05;
                                curSlot.fBottomPercent = -0.06;

                                tradeQueue.Enqueue(curSlot);
                                chanceFloorCntArray[KOSDAQ_ID, 17] = true;
                                testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 코스닥 바닥잡기 매수신청 \r\n"); //++
                            }
                        }// END ---- 바닥잡기 처리

                    } // END ---- 코스닥 한정 전략
                    else if (eachStockArray[nCurIdx].nMarketGubun == KOSPI_ID) // 코스피 한정 전략
                    {
                        if (!chanceFloorCntArray[KOSPI_ID, 15] &&
                            (eachStockArray[nCurIdx].nTime < eachStockArray[nCurIdx].nNoonTime) &&
                             (eachStockArray[nCurIdx].fPower < -0.15))
                        {
                            if (eachStockArray[nCurIdx].nIdx > 200)
                            {
                                curSlot.sRQName = "바닥잡기";
                                curSlot.nOrderType = 1; // 신규매수
                                curSlot.sCode = sCode;
                                curSlot.nEachStockIdx = nCurIdx;
                                curSlot.nOrderPrice = eachStockArray[nCurIdx].nFs;
                                curSlot.sHogaGb = "03";
                                curSlot.sOrgOrderId = "";
                                curSlot.nRqTime = nSharedTime;
                                curSlot.fTargetPercent = 0.06;
                                curSlot.fBottomPercent = -0.06;

                                tradeQueue.Enqueue(curSlot);
                                chanceFloorCntArray[KOSPI_ID, 15] = true;
                                testTextBox.AppendText(eachStockArray[nCurIdx].nTime + " : " + sCode + " 코스피 바닥잡기 매수신청 \r\n"); //++
                            }
                        }// END ---- 바닥잡기 처리
                    } // END ---- 코스피 한정 전략
                }


            }// End ---- e.sRealType.Equals("주식체결")
            else if (e.sRealType.Equals("장시작시간"))
            {

                string sGubun = axKHOpenAPI1.GetCommRealData(e.sRealKey, 215); // 장운영구분 0 :장시작전, 3 : 장중, 4 : 장종료
                if (sGubun.Equals("0")) // 장시작 전
                {
                    testTextBox.AppendText("장시작전\r\n");//++
                }
                else if (sGubun.Equals("3")) // 장 중
                {
                    testTextBox.AppendText("장중\r\n");//++
                    isMarketStart = true;
                    //nFirstDisposal = 0;
                    RequestHoldings(0); // 장시작하고 잔여종목 전량매도
                }
                else
                {
                    if (sGubun.Equals("2")) // 장 종료 10분전 동시호가
                    {
                        testTextBox.AppendText("장종료전\r\n");//++
                        isMarketStart = false;
                        nShutDown++;
                        RequestHoldings(0); // 장 끝나기 전 잔여종목 전량매도
                    }
                    else if (sGubun.Equals("4")) // 장 종료
                    {
                        testTextBox.AppendText("장종료\r\n");//++
                        isMarketStart = false;
                        nShutDown++;
                        isForCheckHoldings = true;
                        RequestHoldings(0);
                        RequestTradeResult();
                    }
                }

            } // End ---- e.sRealType.Equals("장시작시간")
        }

        
        // ==================================================
        // 주식주문(접수, 체결, 잔고) 이벤트발생시 핸들러메소드
        // ==================================================
        private void OnReceiveChejanDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (e.sGubun.Equals("0")) // 접수와 체결 
            { 

                string sTradeTime = axKHOpenAPI1.GetChejanData(908); // 체결시간
                nSharedTime = Math.Abs(int.Parse(sTradeTime)); 

                string sCode = axKHOpenAPI1.GetChejanData(9001).Substring(1); // 종목코드
                int nCodeIdx = int.Parse(sCode);
                nCurIdx = eachStockIdxArray[nCodeIdx];
                int nCurBuySlotIdx = eachStockArray[nCurIdx].nCurBuySlotIdx; // 매수:마지막idx  매도:매도할idx

                string sOrderType = axKHOpenAPI1.GetChejanData(905).Trim(charsToTrim); // +매수, -매도, 매수취소
                string sOrderStatus = axKHOpenAPI1.GetChejanData(913).Trim(); // 주문상태(접수, 체결, 확인)
                string sOrderId = axKHOpenAPI1.GetChejanData(9203).Trim(); // 주문번호
                int nOrderVolume = Math.Abs(int.Parse(axKHOpenAPI1.GetChejanData(900))); // 주문수량
                string sCurOkTradePrice = axKHOpenAPI1.GetChejanData(914).Trim(); // 단위체결가 없을땐 ""
                string sCurOkTradeVolume = axKHOpenAPI1.GetChejanData(915).Trim(); // 단위체결량 없을땐 ""
                int nNoTradeVolume = Math.Abs(int.Parse(axKHOpenAPI1.GetChejanData(902))); // 미체결량

                //string sOkTradePrice = axKHOpenAPI1.GetChejanData(910).Trim(); // 체결가 없을땐 ""
                //string sOkTradeVolume = axKHOpenAPI1.GetChejanData(911).Trim(); // 체결량 없을땐 ""

                // ---------------------------------------------
                // 매수 데이터 수신 순서
                // 매수접수 - 매수체결
                // 매수접수 - (매수취소) - 매수체결
                // 매수접수 - (매수취소) - 매수취소접수 - 매수취소확인 - 매수접수(매수취소확인)
                // 매수접수 - (매수취소) - 매수체결 - 매수취소접수 - 매수취소확인 - 매수체결(매수취소확인)
                // ---------------------------------------------
                if (sOrderType.Equals("매수"))
                {
                    if (sOrderStatus.Equals("체결"))
                    {
                        // 매수-체결됐으면 3가지로 나눠볼 수 있는데
                        // 1. 일반적으로 일부 체결된 경우
                        // 2. 전량 체결된 경우
                        // 3. 일부 체결된 후 나머지는 매수취소된 경우(미체결 클리어를 위해 얻어지는 경우)



                        // 문자열로 받아진 단위체결량과 단위체결가를 정수로 바꾸는 작업을 한다.
                        // 접수나 취소 때는 체결가~ 종류는 "" 공백으로 받아지기 때문에
                        // 정수 캐스팅을 하면 오류가 나기 때문이다
                        int nCurOkTradeVolume;
                        int nCurOkTradePrice;
                        try
                        {
                            nCurOkTradeVolume = Math.Abs(int.Parse(sCurOkTradeVolume)); // n단위체결량
                            nCurOkTradePrice = Math.Abs(int.Parse(sCurOkTradePrice)); // n단위체결가
                        }
                        catch (Exception ex)
                        {
                            // 혹시 문자열이 ""이라면 매수체결시 받아지는 체결 메시지다.
                            eachStockArray[nCurIdx].isCancelComplete = false; // 매수취소완료버튼 초기화
                            eachStockArray[nCurIdx].isCancelMode = false; // 해당종목의 현재매수취소버튼 초기화
                            buySlotArray[nCurIdx, nCurBuySlotIdx].isAllBuyed = true; // 해당종목의 매수레코드의 매수완료 on
                            buySlotCntArray[nCurIdx]++; // 매수레코드 수 증가
                            eachStockArray[nCurIdx].nBuyReqCnt--; // 매수요청 카운트 감소
                            eachStockArray[nCurIdx].isOrderStatus = false; // 매매중 off
                            return;
                        }
                        // 예수금에 지정상한가와 매입금액과의 차이만큼을 다시 복구시켜준다.
                        nCurDeposit += (eachStockArray[nCurIdx].nCurLimitPrice - nCurOkTradePrice) * nCurOkTradeVolume; // 예수금에 (추정매수가 - 실매수가) * 실매수량 더해준다. //**

                        // 이것은 현재매수 구간이기 떄문에
                        // 해당레코드의 평균매입가와 매수수량을 조정하기 위한 과정이다
                        int sum = buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyPrice * buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyVolume;
                        sum += nCurOkTradePrice * nCurOkTradeVolume;
                        buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyVolume += nCurOkTradeVolume;
                        buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyPrice = (int)(sum / buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyVolume);

                        if (nNoTradeVolume == 0) // 매수 전량 체결됐다면
                        {
                            // 매수가 전량 체결됐다면 
                            // 체결-매수취소와 유사하게 진행된다 하나 다른점은 매수취소완료 시그널을 건들 필요가 없다는 것이다.
                            // 현재매수취소 그리고 일부라도 체결됐으니 해당레코드에 구매됐다는 시그널을 on해주고 레코드인덱스를 한칸 늘린다
                            // 매수요청 카운트도 낮추고 현재 매매중인 시그널을 off해준다.
                            eachStockArray[nCurIdx].isCancelMode = false; // 매수취소를 했어도 취소접수가 안되면 그대로 전량체결이 되니까 이때 cancelMode를 false한다.
                            buySlotArray[nCurIdx, nCurBuySlotIdx].isAllBuyed = true; // 해당종목의 매수레코드의 매수완료 on
                            buySlotCntArray[nCurIdx]++; // 매수레코드 수 증가
                            eachStockArray[nCurIdx].nBuyReqCnt--; // 매수요청 카운트 감소
                            eachStockArray[nCurIdx].isOrderStatus = false; // 매매중 off

                            testTextBox.AppendText(sTradeTime + " : " + sCode + " 매수 체결완료 \r\n"); //++
                        }
                    }
                    else if (sOrderStatus.Equals("접수"))
                    {
                        if (nNoTradeVolume == 0) // 전량 매수취소가 완료됐다면
                        {
                            // 접수-매수취소는
                            // 체결이 하나도 안된상태에서 매수주문이 모두 매수취소 된 상황이다
                            // 많은 설정을 할 필요가 없다
                            // 여기서는 isAllBuyed와 현재레코드인덱스를 더하지 않는 이유는 체결데이터가 없기때문에
                            // 굳이 인덱스를 늘려 레코드만 증가시킨다면 실시간에서 관리함에 시간이 더 소요되기 때문이다
                            eachStockArray[nCurIdx].isCancelComplete = false; // 매수취소완료버튼 초기화
                            eachStockArray[nCurIdx].isCancelMode = false; // 해당종목의 현재매수취소버튼 초기화
                            eachStockArray[nCurIdx].nBuyReqCnt--; // 매수요청 카운트 감소
                            eachStockArray[nCurIdx].isOrderStatus = false; // 매매중 off
                        }
                        else // 매수 주문인경우
                        {
                            // 원주문번호만 설정해준다.
                            
                            eachStockArray[nCurIdx].sCurOrgOrderId = sOrderId; // 현재원주문번호 설정
                            eachStockArray[nCurIdx].sCurOrgBuyId = sOrderId; // 매수취소,정정용 원주문번호
                            eachStockArray[nCurIdx].isOrderStatus = true; // 매매중 on

                            nCurDeposit -= (int)(nOrderVolume * eachStockArray[nCurIdx].nCurLimitPrice * (1 + VIRTUAL_STOCK_FEE)); // 예수금에서 매매수수료까지 포함해서 차감

                            testTextBox.AppendText(sTradeTime + " : " + sCode +", "+ nOrderVolume + "(주) 매수 접수완료 \r\n"); //++
                            //---------------------------------------------
                            // 구매기록 초기화
                            // --------------------------------------------
                            // 여기서 퍼센트는 매수,매도 시 curSlot에 설정하고
                            // 매매컨트롤러에서 eachStockArray에 설정하는 과정을 거쳐
                            // buySlotArray에 설정되는 과정으로 마쳐진다.
                            buySlotArray[nCurIdx, nCurBuySlotIdx].isSelled = false;
                            buySlotArray[nCurIdx, nCurBuySlotIdx].isAllBuyed = false;
                            buySlotArray[nCurIdx, nCurBuySlotIdx].fTargetPer = eachStockArray[nCurIdx].fTargetPercent;
                            buySlotArray[nCurIdx, nCurBuySlotIdx].fBottomPer = eachStockArray[nCurIdx].fBottomPercent;
                            buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyPrice = 0;
                            buySlotArray[nCurIdx, nCurBuySlotIdx].nBuyVolume = 0;
                        }
                    }
                } // END ---- orderType.Equals("매수")
                else if(sOrderType.Equals("매도"))
                {
                    if (sOrderStatus.Equals("체결")) 
                    {
                        nCurDeposit += (int)(Math.Abs(int.Parse(sCurOkTradeVolume)) * Math.Abs(int.Parse(sCurOkTradePrice)) * (1-(STOCK_TAX + VIRTUAL_STOCK_FEE))); //**

                        if (nNoTradeVolume == 0)
                        {
                            if (eachStockArray[nCurIdx].nSellReqCnt > 0) //** 아침에 어제 매도 안된 애들이 남아있으면 sellReqCnt가 음수가 될 수 도 있으니 0이 넘어야만 차감되게끔 한다.
                                eachStockArray[nCurIdx].nSellReqCnt--;

                            eachStockArray[nCurIdx].isOrderStatus = false; // 매매중 off
                            testTextBox.AppendText(sTradeTime + " : " + sCode +  " 매도 체결완료 \r\n"); //++
                        }
                    }
                    else if (sOrderStatus.Equals("접수")) 
                    {
                        testTextBox.AppendText(sTradeTime + " : " + sCode + ", " + nOrderVolume + "(주) 매도 접수완료 \r\n"); //++
                        eachStockArray[nCurIdx].isOrderStatus = true; // 매매중 on
                        eachStockArray[nCurIdx].sCurOrgOrderId = sOrderId; // 원주문번호
                        eachStockArray[nCurIdx].sCurOrgSellId = sOrderId; // 매도취소,정정용 원주문번호
                    }
                } // END ---- orderType.Equals("매도")
                else if(sOrderType.Equals("매수취소")) 
                {
                    // ----------------------------------
                    // 야기할 수 있는 문제
                    // 1. 매수취소확인후 접수,체결을 안보내준다.
                    // 2. 매수취소확인전에 접수,체결을 보내준다.
                    // ----------------------------------

                    // 매수취소에서는 매수취소완료버튼 on
                    // 매수취소수량이 있으면 그만큼 예수금 더해주면 된다
                    // 거래중, 매매완료 등등의 처리는 매수에서 완료한다.
                    if (sOrderStatus.Equals("접수")) 
                    {
                        testTextBox.AppendText(sTradeTime + " : " + sCode + ", " + nOrderVolume + "(주) 매수취소 접수완료 \r\n"); //++
                        // 매수취소 접수가 되면 거의 확정적으로 매수취소확인 따라오며 
                        // 매수취소 접수때부터 이미 매수취소된거같음.
                    }
                    else if (sOrderStatus.Equals("확인")) 
                    {
                        eachStockArray[nCurIdx].isCancelComplete = true; // 매수취소 완료

                        // 매수취소확인은 사실상 매수취소 수량이 있는거고 미체결량은 0인 상태일 테지만 
                        // 예기치 못한 오류로 인해 문제가 생길 수 도 있으니
                        // 매수취소 수량과 미체결량을 검사해준다.
                        if ( nNoTradeVolume < nOrderVolume  && nOrderVolume > 0) // 매수취소된 수량이 있다면
                        {
                            nCurDeposit += (int)((nOrderVolume - nNoTradeVolume) * (eachStockArray[nCurIdx].nCurLimitPrice * (1 + VIRTUAL_STOCK_FEE)));
                        }
                        
                    }
                } // END ---- orderType.Equals("매수취소")

            } // End ---- e.sGubun.Equals("0") : 접수,체결

            else if(e.sGubun.Equals("1")) // 잔고
            {
                string sCode = axKHOpenAPI1.GetChejanData(9001).Substring(1); // 종목코드
                int nCodeIdx = Math.Abs(int.Parse(sCode));
                nCurIdx = eachStockIdxArray[nCodeIdx];

                int nHoldingQuant = Math.Abs(int.Parse(axKHOpenAPI1.GetChejanData(930))); // 보유수량
                eachStockArray[nCurIdx].nHoldingsCnt = nHoldingQuant;
            } // End ---- e.sGubun.Equals("1") : 잔고
        }

    }
}
