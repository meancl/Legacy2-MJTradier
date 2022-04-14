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

namespace MJTradier
{ 
    public partial class Form1 : Form
    {
        
        // ------------------------------------------------------
        // 상수 변수
        // ------------------------------------------------------
        private const byte KOSDAQ_NUM = 1;
        private const byte KOSPI_NUM = 2;
        private const int BILLION = 100000000;
        private const int MAX_STOCK_NUM = 1000000;
        public const int NUM_SEP_PER_SCREEN = 100;
        public const double STOCK_FEE = 0.0026;
        public const int MAX_STOCK_HOLDINGS_NUM = 200;

        // ------------------------------------------------------
        // 각 종목 구조체 변수
        // ------------------------------------------------------
        private int eachStockIdx;
        private int[] eachStockIdxArray = new int[MAX_STOCK_NUM];
        EachStock[] eachStockArray;  // 각 주식이 가지는 실시간용 구조체

        // ------------------------------------------------------
        // 기타 변수
        // ------------------------------------------------------
        public int curIdx;

        // ------------------------------------------------------
        // 스크린번호 변수
        // ------------------------------------------------------
        private int screenNum = 1000;
        private string strScreenNum;

        // ------------------------------------------------------
        // 종목획득 변수
        // ------------------------------------------------------
        private string configurationPath = @"G:\getData\kiwoom\";
        private string[] kosdaqCodes;
        private string[] kospiCodes;


        // ------------------------------------------------------
        // 공유 변수
        // ------------------------------------------------------
       
        public bool marketStart; // true면 장중, false면 장시작전 
        public string accountNum; // 나의 계좌번호
        public bool lockDeposit;  // 예수금 계산의 신뢰성을 보장하기 위해 lockDeposit이 false일때만 신규매수가 가능하게 했다.
        public string lockCode; // 삭제예정/ 현재 매수가 진행 중인 종목코드
        public int sharedTime; // 모든 종목들이 공유하는 현재시간


        // ------------------------------------------------------
        // 매매관련 변수
        // ------------------------------------------------------
        public int curDeposit;  // 햔제 예수금
        public int ceilPrice;   // 총매수가능 상한가
        public Queue<TradeSlot> tradeQueue = new Queue<TradeSlot>(); // 모든 매매신청을 담는 큐
        TradeSlot curSlot; // 임시로 사용하능한 매매신청 구조체변수

        //--------------------------------------------------------
        // 계좌평가잔고내역요청 변수
        //--------------------------------------------------------
        public Holdings[] holdingsArray = new Holdings[MAX_STOCK_HOLDINGS_NUM];
        public int holdingCnt;
        public int curHoldingsIdx;

        public Form1()
        {
            InitializeComponent(); // c# 고유 고정메소드  

            MappingFileToStockCodes();


            // --------------------------------------------------
            // Winform Event Handler 
            // --------------------------------------------------
            loginToolStripMenuItem.Click += ToolStripMenuItem_Click;
            checkMyAccountInfoButton.Click += Button_Click;
            checkMyHoldingsButton.Click += Button_Click;
            
            // --------------------------------------------------
            // Event Handler 
            // --------------------------------------------------
            axKHOpenAPI1.OnEventConnect += OnEventConnectHandler; // 로그인 event slot connect
            axKHOpenAPI1.OnReceiveTrData += OnReceiveTrDataHandler; // TR event slot connect
            axKHOpenAPI1.OnReceiveRealData += OnReceiveRealDataHandler; // 실시간 event slot connect
            axKHOpenAPI1.OnReceiveChejanData += OnReceiveChejanDataHandler; // 체결,접수,잔고 event slot connect

        }



        
        // ============================================
        // 주식종목들을 특정 txt파일에서 읽어
        // 코스닥, 코스피 변수에 string[] 형식으로 각각 저장
        // 코스닥, 코스피 종목갯수의 합만큼의 eachStockArray구조체 배열을 생성
        // ============================================
        private void MappingFileToStockCodes()
        {
            kosdaqCodes = System.IO.File.ReadAllLines(configurationPath + "today_kosdaq_stock_code.txt");
            kospiCodes = System.IO.File.ReadAllLines(configurationPath + "today_kospi_stock_code.txt");
            eachStockArray = new EachStock[kosdaqCodes.Length + kospiCodes.Length];
        }



       
        // ============================================
        // string형  코스닥, 코스피 종목코드의 배열 string[n] 변수에서
        // 한 화면번호 당 (최대)100개씩 넣고 주식체결 fid를 넣고
        // 실시간 데이터 요청을 진행
        // 코스닥과 코스피 배열에서 100개가 안되는 나머지 종목들은 코스닥,코스피 각 다른 화면번호에 실시간 데이터 요청
        // ============================================
        private void SubscribeRealData()
        {
            
            int kosdaqIndex = 0;
            int kosdaqCodesLength = kosdaqCodes.Length;
            int kosdaqIterNum = kosdaqCodesLength / NUM_SEP_PER_SCREEN;
            int kosdaqRestNum = kosdaqCodesLength % NUM_SEP_PER_SCREEN;
            string strKosdaqCodeList;
            const string sFID ="228" ; // 체결강도. 실시간 목록 FID들 중 겹치는게 가장 적은 FID

            // ------------------------------------------------------
            // 코스닥 실시간 등록
            // ------------------------------------------------------
            // 100개 단위
            for (int i = 0; i< kosdaqIterNum; i++)
            {
                SetScreenNo();
                strKosdaqCodeList = ConvertStrCodeList(kosdaqCodes, kosdaqIndex, kosdaqIndex + NUM_SEP_PER_SCREEN, KOSDAQ_NUM);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKosdaqCodeList, sFID, "0");
                kosdaqIndex += NUM_SEP_PER_SCREEN;
            }
            // 나머지
            if (kosdaqRestNum > 0)
            {
                SetScreenNo();
                strKosdaqCodeList = ConvertStrCodeList(kosdaqCodes, kosdaqIndex, kosdaqIndex + kosdaqRestNum, KOSDAQ_NUM);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKosdaqCodeList, sFID, "0");
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
                SetScreenNo();
                strKospiCodeList = ConvertStrCodeList(kospiCodes, kospiIndex, kospiIndex + NUM_SEP_PER_SCREEN, KOSPI_NUM);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKospiCodeList, sFID, "0");
                kospiIndex += NUM_SEP_PER_SCREEN;
            }
            // 나머지
            if (kospiRestNum > 0)
            {
                SetScreenNo();
                strKospiCodeList = ConvertStrCodeList(kospiCodes, kospiIndex, kospiIndex + kospiRestNum, KOSPI_NUM);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKospiCodeList, sFID, "0");
            }
        }




        // ============================================
        // 매개변수 : 
        //  1.  string[] codes : 주식종목코드 배열
        //  2.  s : 배열의 시작 인덱스
        //  3.  e : 배열의 끝 인덱스 (포함 x)
        //  
        // 키움 실시간 신청메소드의 두번째 인자인 strCodeList는
        // 종목코드1;종목코드2;종목코드3;....;종목코드n(;마지막은 생략가능) 형식으로 넘겨줘야하기 때문에
        // s부터 e -1 인덱스까지 string 변수에 추가하며 사이사이 ';'을 붙여준다
        //
        // 실시간메소드에서 각 종목의 구조체를 사용하기 위해 초기화과정이 필요한데
        // 이 메소드에서 같이 진행해준다.
        // ============================================
        private string ConvertStrCodeList(string[] codes, int s, int e, int marketGubun)
        {
            string strKosdaqCodeList = "";
            for (int i = s; i < e; i++)
            {
                int codeIdx = int.Parse(codes[i]);

                // TODO. Map(java) 기능과 속도 비교 후 수정 예정
                ////// eachStockIdx 설정 부분 ///////
                eachStockIdxArray[codeIdx] = eachStockIdx;
                eachStockIdx++;
                /////////////////////////////////////

                ////// eachStock 초기화 부분 //////////
                curIdx = eachStockIdxArray[codeIdx];
                eachStockArray[curIdx].screenNum = strScreenNum;
                eachStockArray[curIdx].code = codes[i];
                eachStockArray[curIdx].initMode = true; // 삭제예정
                eachStockArray[curIdx].maxPower = -100.0;
                eachStockArray[curIdx].marketGubun = marketGubun;
                //////////////////////////////////////

                strKosdaqCodeList += codes[i];
                if (i < e - 1)
                    strKosdaqCodeList += ';';
            }
            return strKosdaqCodeList;
        }




        // ============================================
        // StripMenuItem을 이용해
        // 메뉴 -> 로그인 버튼을 클릭하면 로그인 창이 뜸
        // 자동로그인 기능을 켜놨다면 로그인이 자동으로 진행됨.
        // ============================================
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(sender.Equals(loginToolStripMenuItem))
            {
                axKHOpenAPI1.CommConnect();
            }
        }



        // ============================================
        // 초기화면번호는 1000번 
        // 메소드를 실행하면 strScreenNum 문자열변수에 string화된 화면번호를 저장 후
        // 기존화면번호를 한칸 올린다.
        // 화면번호는 4자리기 때문에 9999를 초과하면 1000번으로 되돌린다.(만약의 상황을 대비)
        // ============================================
        private void SetScreenNo()
        {
            if (screenNum > 9999)
                screenNum = 1000;

            strScreenNum = screenNum.ToString();
            screenNum++;
        }

        // ============================================
        // 계좌평가잔고내역요청 TR요청메소드
        // CommRqData 3번째 인자 sPrevNext가 0일 경우 처음 20개의 종목을 요청하고
        // 2일 경우 초기20개 초과되는 종목들을 계속해서 요청한다.
        // ============================================
        private void RequestHoldings(int sPrevNext)
        {
            holdingCnt = 0;
            SetScreenNo();
            axKHOpenAPI1.SetInputValue("계좌번호", accountComboBox.Text);
            axKHOpenAPI1.SetInputValue("비밀번호", "");
            axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
            axKHOpenAPI1.SetInputValue("조회구분", "2"); // 1:합산 2:개별
            axKHOpenAPI1.CommRqData("계좌평가잔고내역요청", "opw00018", sPrevNext, strScreenNum);
        }


        // ============================================
        // 버튼 클릭 이벤트의 핸들러 메소드
        // 1. 예수금상세현황요청
        // 2. 계좌평가잔고내역요청
        // ============================================
        private void Button_Click(object sender, EventArgs e)
        {
            
            if (sender.Equals(checkMyAccountInfoButton)) // 예수금상세현황요청
            {
                SetScreenNo();
                axKHOpenAPI1.SetInputValue("계좌번호", accountComboBox.Text);
                axKHOpenAPI1.SetInputValue("비밀번호", "");
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
                axKHOpenAPI1.SetInputValue("조회구분", "2");
                axKHOpenAPI1.CommRqData("예수금상세현황요청", "opw00001", 0, strScreenNum);
            }
            else if(sender.Equals(checkMyHoldingsButton)) // 계좌평가현황요청 
            {
                curHoldingsIdx = 0;
                RequestHoldings(0);
            }
        }



        // ============================================
        // TR 이벤트발생시 핸들러 메소드
        // ============================================
        private void OnReceiveTrDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sRQName.Equals("예수금상세현황요청"))
            {
                int iMyDeposit = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, 0, "예수금"));
                curDeposit = iMyDeposit;
                myDepositLabel.Text = iMyDeposit.ToString();
            }
            else if(e.sRQName.Equals("계좌평가잔고내역요청"))
            {
                int rows = axKHOpenAPI1.GetRepeatCnt(e.sTrCode, e.sRecordName);
                holdingCnt += rows;

                for (int i =0; curHoldingsIdx < holdingCnt; curHoldingsIdx++, i++)
                {
                    holdingsArray[curHoldingsIdx].code = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "종목번호");
                    holdingsArray[curHoldingsIdx].codeName = axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "종목명");
                    holdingsArray[curHoldingsIdx].yield = double.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "수익률(%)"));
                    holdingsArray[curHoldingsIdx].holdingQty = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "보유수량")));
                    holdingsArray[curHoldingsIdx].buyedPrice = Math.Abs(int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, i, "매입금액")));
                }

                if (e.sPrevNext.Equals("2"))
                {
                    RequestHoldings(2);
                }
                else // 보유잔고 확인 끝
                {
                    // TODO
                }
            }
        }



       
        // ============================================
        // 로그인 이벤트발생시 핸들러 메소드
        // ============================================
        private void OnEventConnectHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0) // 로그인 성공
            {
                
                string myName = axKHOpenAPI1.GetLoginInfo("USER_NAME");
                string accList = axKHOpenAPI1.GetLoginInfo("ACCLIST"); // 로그인 사용자 계좌번호 리스트 요청
                string[] accountArray = accList.Split(';');
                SubscribeRealData();
                foreach (string account in accountArray)
                {
                    if (account.Length > 0)
                        accountComboBox.Items.Add(account);
                }
                myNameLabel.Text = myName + "(원)";
            }
            else
            {
                MessageBox.Show("로그인 실패");
            }
        }


        // ============================================
        // 실시간 이벤트발생시 핸들러메소드 
        // ============================================
        private void OnReceiveRealDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (e.sRealType.Equals("주식체결"))
            {

                string code = e.sRealKey;

                // -------------------------------------------------------------------------
                // 체결 요청
                // 매매컨트롤러 부분은 Async를 사용하려 했는데
                // 오버헤드가 클거같아서 실시간 부분에 끼워넣었다.
                // -------------------------------------------------------------------------
                // TODO
                // SendOrder 부분 구현해야 함.
                if (tradeQueue.Count > 0)
                {
                    curSlot = tradeQueue.Peek();


                    if (SubTimeToTime(sharedTime, curSlot.nRqTime) <= 2) // 요청 시간 - 현재시간 < 2초
                    {
                        if (curSlot.nOrderType == 1) // 신규매수
                        {
                            if (!lockDeposit)
                            {
                                if (curSlot.sHogaGb.Equals("00")) // 지정가
                                {

                                }
                                else if (curSlot.sHogaGb.Equals("03")) // 시장가
                                {
                                    lockCode = code; // 삭제예정
                                    lockDeposit = true;
                                    axKHOpenAPI1.SendOrder(curSlot.sRQName, curSlot.sScreenNo, curSlot.sAccNo,
                                        curSlot.nOrderType, curSlot.sCode, curSlot.nQty, curSlot.nPrice,
                                        curSlot.sHogaGb, curSlot.sOrgOrderNo);
                                }
                                tradeQueue.Dequeue();
                            }
                        }
                        else
                        {
                            tradeQueue.Dequeue();
                            axKHOpenAPI1.SendOrder(curSlot.sRQName, curSlot.sScreenNo, curSlot.sAccNo,
                                        curSlot.nOrderType, curSlot.sCode, curSlot.nQty, curSlot.nPrice,
                                        curSlot.sHogaGb, curSlot.sOrgOrderNo);

                            if (curSlot.nOrderType == 2) // 신규매도
                            {

                            }
                            else if (curSlot.nOrderType == 3) // 매수취소
                            {

                            }
                            else if (curSlot.nOrderType == 4) // 매도취소
                            {

                            }
                            else if (curSlot.nOrderType == 5) // 매수정정
                            {

                            }
                            else if (curSlot.nOrderType == 6) // 매도정정
                            {

                            }
                        }
                    }
                    else
                    {
                        // 처분
                        tradeQueue.Dequeue();
                    }

                }

                // -------------------------------------------------------------------------
                // 실시간 데이터 처리
                // -------------------------------------------------------------------------

                int codeIdx = int.Parse(code);
                curIdx = eachStockIdxArray[codeIdx];

                if (!marketStart || eachStockArray[curIdx].passMode)
                    return;


                eachStockArray[curIdx].time = sharedTime = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 20))); // 현재시간
                eachStockArray[curIdx].fs = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 27))); // 최우선매도호가
                eachStockArray[curIdx].fb = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 28))); // 최우선매수호가
                eachStockArray[curIdx].tv = int.Parse(axKHOpenAPI1.GetCommRealData(code, 15));
                eachStockArray[curIdx].accumTradeQnt = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 13)));
                eachStockArray[curIdx].accumTradePrice = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 14)));

                eachStockArray[curIdx].idx++;

                // TODO.
                // -----------------------------------------------
                // 장종료시간이 다 됐고 현재 종목을 들고있다면 전량매도 진행
                // -----------------------------------------------
                


                if (!eachStockArray[curIdx].firstCheck)
                {
                    if (eachStockArray[curIdx].firstTime == 0) // 처음일때 시간설정
                    {
                        eachStockArray[curIdx].firstTime = eachStockArray[curIdx].time;
                        eachStockArray[curIdx].tenTime = AddTimeBySec(eachStockArray[curIdx].firstTime, 3600);
                        eachStockArray[curIdx].noonTime = 120000;
                    }

                    if (eachStockArray[curIdx].fs == 0)
                        return;
                    else
                    {
                        if (eachStockArray[curIdx].fb == 0)
                            eachStockArray[curIdx].fb = GetKosdaqGap(eachStockArray[curIdx].fs - 1);

                        eachStockArray[curIdx].firstPrice = eachStockArray[curIdx].fs;

                        if (eachStockArray[curIdx].fs < 1000)
                        {
                            // 실시간 제거대상
                            eachStockArray[curIdx].passMode = true;
                            axKHOpenAPI1.SetRealRemove(eachStockArray[curIdx].screenNum, eachStockArray[curIdx].code);
                        }
                        eachStockArray[curIdx].firstCheck = true;
                    }
                }
                eachStockArray[curIdx].power = (eachStockArray[curIdx].fb - eachStockArray[curIdx].firstPrice) / eachStockArray[curIdx].firstPrice;

                // 이상 데이터 감지
                if ((eachStockArray[curIdx].fs - eachStockArray[curIdx].fb) / eachStockArray[curIdx].fb > 0.02)
                    return;

                // -----------------------------------------
                // 전고점 부분 처리
                // -----------------------------------------
                if (eachStockArray[curIdx].crushCount <= 1 && SubTimeToTime(eachStockArray[curIdx].minTime, eachStockArray[curIdx].maxTime) >= 1 &&
                    SubTimeToTime(eachStockArray[curIdx].time, eachStockArray[curIdx].maxTime) >= 1 &&
                        (eachStockArray[curIdx].maxPower - eachStockArray[curIdx].minPower) > 0.01 &&
                        eachStockArray[curIdx].power > eachStockArray[curIdx].maxPower && eachStockArray[curIdx].time < 144000)
                {
                    eachStockArray[curIdx].crushCount++;
                    if (eachStockArray[curIdx].crushCount == 1 && eachStockArray[curIdx].power >= 0.1 && eachStockArray[curIdx].power <= 0.15)
                    {
                        // 구매해야 함


                        lockDeposit = true;
                        SetScreenNo();
                        curSlot.sRQName = "전고점돌파";
                        curSlot.sScreenNo = strScreenNum;
                        curSlot.sAccNo = accountNum;
                        curSlot.nOrderType = 1;
                        curSlot.sCode = code;
                        curSlot.nQty = 0;
                        curSlot.nPrice = 0;
                        curSlot.sHogaGb = "03";
                        curSlot.sOrgOrderNo = eachStockArray[curIdx].orgOrderNo;

                        tradeQueue.Enqueue(curSlot);

                    }
                }

                if (eachStockArray[curIdx].maxPower < eachStockArray[curIdx].power)
                {
                    eachStockArray[curIdx].maxPower = eachStockArray[curIdx].power;
                    eachStockArray[curIdx].maxTime = eachStockArray[curIdx].time;
                    eachStockArray[curIdx].minPower = eachStockArray[curIdx].power;
                    eachStockArray[curIdx].minTime = eachStockArray[curIdx].time;
                }

                if (eachStockArray[curIdx].minPower > eachStockArray[curIdx].power)
                {
                    eachStockArray[curIdx].minPower = eachStockArray[curIdx].power;
                    eachStockArray[curIdx].minTime = eachStockArray[curIdx].time;
                }


                // -----------------------------------------
                // 바닥잡기 부분 처리
                // -----------------------------------------
                if (eachStockArray[curIdx].time < eachStockArray[curIdx].noonTime && eachStockArray[curIdx].power < -0.17)
                {
                    if (eachStockArray[curIdx].idx > 1000)
                    {
                        // TODO
                        // 구매해야 함
                    }
                    else
                    {
                        // TODO
                        // 구매 안하고 종료
                    }
                }
            } // End ---- e.sRealType.Equals("주식체결")


            else if(e.sRealType.Equals("장시작시간"))
            {
                if (marketStart) // 실시간해지 신청을 했음에도 잔여시간동안 메시지가 올때를 대비한 조건문
                    return;

                string sGubun = axKHOpenAPI1.GetCommRealData(e.sRealKey, 215); // 장운영구분 0 :장시작전, 3 : 장중, 4 : 장종료
                if (sGubun.Equals("0")) // 장시작 전
                {
                    testListView.Items.Add("장시작전");
                }
                else if (sGubun.Equals("3")) // 장 중
                {
                    testListView.Items.Add("장중");
                    marketStart = true;
                }
                else 
                {
                    if (sGubun.Equals("2")) // 장 종료 10분전 동시호가
                    {
                        // TODO
                        // -------------------------------------
                        // 보유종목이 아직 남아있다면
                        // -------------------------------------
                        // 보유종목 전량 매도
                        marketStart = false;
                    }
                    else if (sGubun.Equals("4")) // 장 종료
                    {
                        marketStart = false;
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
                
                string tradeTime = axKHOpenAPI1.GetChejanData(908); // 체결시간
                string orderNum = axKHOpenAPI1.GetChejanData(9203); // 주문번호
                string code = axKHOpenAPI1.GetChejanData(9001); // 종목코드
                int codeIdx = int.Parse(code.Substring(1));
                curIdx = eachStockIdxArray[codeIdx];
                string orderTaskCategory = axKHOpenAPI1.GetChejanData(912); // 주문업무분류
                string orderStatus = axKHOpenAPI1.GetChejanData(913); // 주문상태(접수, 체결)
                if (orderStatus.Equals("접수"))
                {
                    eachStockArray[curIdx].orgStatus = true;
                    eachStockArray[curIdx].orgOrderNo = orderNum;
                    return;
                }
                string codeName = axKHOpenAPI1.GetChejanData(302);  // 종목명
                string orderType = axKHOpenAPI1.GetChejanData(905); // +매수, -매도
                string orderQuant = axKHOpenAPI1.GetChejanData(900); // 주문수량
                string orderPRice = axKHOpenAPI1.GetChejanData(901); // 주문가격
                int okTradePrice = Math.Abs(int.Parse(axKHOpenAPI1.GetChejanData(910))); // 체결가
                int okTradeQuant = Math.Abs(int.Parse(axKHOpenAPI1.GetChejanData(911))); // 체결량
                string noTradeQuant = axKHOpenAPI1.GetChejanData(902); // 미체결량
                
                sharedTime = Math.Abs(int.Parse(tradeTime));

                

                if (orderStatus.Equals("체결"))
                {
                    if (eachStockArray[curIdx].nOrderType == 1) // 신규매수
                    {
                        // -----------------------------------
                        // 사는 데 시간이 오래걸린다면 처리
                        // 상수는 임시로 지정함.
                        // -----------------------------------
                        if (SubTimeToTime(sharedTime, eachStockArray[curIdx].nOrderTime) >= 5)
                        {
                            // TODO
                            // 기입 예정
                        }

                        // -----------------------------------
                        // 너무 높은 금액에 사지고 있다면 처리
                        // 상수는 임시로 지정함.
                        // -----------------------------------
                        if (((okTradePrice - eachStockArray[curIdx].nOrderPrice) / eachStockArray[curIdx].nOrderPrice) >= 0.015)
                        {
                            // TODO
                            // 기입 예정
                        }


                        if (noTradeQuant.Equals("0")) // 전량 매수시
                        {
                            lockDeposit = false;
                            eachStockArray[curIdx].orgStatus = false;
                            eachStockArray[curIdx].orgOrderNo = "";
                            eachStockArray[curIdx].bTradeEnds = true;
                            
                        }

                        if (eachStockArray[curIdx].sellMode == 1) // 지정가 매도시
                        {
                            // TODO
                            // --------------------------------------------
                            // 지정가인 경우 가격을 정확하게 기입해야 함.
                            // 기입 예정
                            // --------------------------------------------

                        }
                    }
                    else if (eachStockArray[curIdx].nOrderType == 2) // 신규매도
                    {
                        if (noTradeQuant.Equals("0")) // 전량 매도시
                        {
                            eachStockArray[curIdx].orgStatus = false;
                            eachStockArray[curIdx].orgOrderNo = "";
                            eachStockArray[curIdx].bTradeEnds = true;
                        }
                    }
                    else if (eachStockArray[curIdx].nOrderType == 3) // 매수취소
                    {

                    }
                    else if (eachStockArray[curIdx].nOrderType == 4) // 매도취소
                    {

                    }
                    else if (eachStockArray[curIdx].nOrderType == 5) // 매수정정
                    {

                    }
                    else if (eachStockArray[curIdx].nOrderType == 6) // 매도정정
                    {

                    }
                } // End ---- orderStatus.Equals("체결")


            } // End ---- e.sGubun.Equals("0") : 접수,체결

            else if(e.sGubun.Equals("1")) // 잔고
            {
    
                string code = axKHOpenAPI1.GetChejanData(9001); // 종목코드
                int codeIdx = Math.Abs(int.Parse(code.Substring(1)));
                curIdx = eachStockIdxArray[codeIdx];
                string codeName = axKHOpenAPI1.GetChejanData(302); // 종목코드
                string curPrice = axKHOpenAPI1.GetChejanData(10); // 현재가
                string curQuant = axKHOpenAPI1.GetChejanData(930); // 보유수량
                int tradePrice = Math.Abs(int.Parse(axKHOpenAPI1.GetChejanData(931))); // 매입단가
                string orderPossibleQuant = axKHOpenAPI1.GetChejanData(933); // 주문가능수량

                
                if (eachStockArray[curIdx].nOrderType == 1) // 신규매수
                {
                    if (eachStockArray[curIdx].bTradeEnds) // 매수체결이 모두 완료됐을 때
                    {
                        eachStockArray[curIdx].bTradeEnds = false;
                        eachStockArray[curIdx].nBuyedPrice = tradePrice; // 시장가매도 변수
                      
                    }
                    else
                    {
                        eachStockArray[curIdx].nBuyingPrice = tradePrice; // 모두 체결이 되지 않았지만 그래도 매입단가
                    }
                }
                else if (eachStockArray[curIdx].nOrderType == 2) // 신규매도
                {
                    if (eachStockArray[curIdx].bTradeEnds) // 매도체결이 모두 완료됐을 때
                    {
                        eachStockArray[curIdx].bTradeEnds = false;
                    }
                }
            } // End ---- e.sGubun.Equals("1") : 잔고

            else if(e.sGubun.Equals("4")) // 파생잔고
            {

            } // End ---- e.sGubun.Equals("4") : 파생잔고
            else
            {
                Console.WriteLine("OnReceiveChejanDataHandler 해당 sGubun값이 존재하지 않음" + e.sGubun);
            } 
        }


    }
}
