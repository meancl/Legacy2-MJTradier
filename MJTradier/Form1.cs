﻿using System;
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
    
        private const int BILLION = 100000000;
        private const int MAX_STOCK_NUM = 1000000;
        private short eachStockIdx;
        private short[] eachStockIdxArray = new short[MAX_STOCK_NUM];
        EachStock[] eachStockArray;  // 각 주식이 가지는 실시간용 구조체
        EachStock curStock; 

        private int screenNum = 1000;
        private string strScreenNum;
        private string configurationPath = @"G:\getData\kiwoom\";
        private string[] kosdaqCodes;
        private string[] kospiCodes;
        public int nSepPerScreen = 100;
        public bool marketStart; // true면 장중, false면 장시작전 
        public string accountNum; // 나의 계좌번호
        
        public bool lockDeposit;  // 예수금 계산의 신뢰성을 보장하기 위해 lockDeposit이 false일때만 신규매수가 가능하게 했다.
        public string lockCode; // 삭제예정/ 현재 매수가 진행 중인 종목코드
        public int sharedTime; // 모든 종목들이 공유하는 현재시간
        public Queue<TradeSlot> tradeQueue = new Queue<TradeSlot>(); // 모든 매매신청을 담는 큐
        TradeSlot curSlot; // 임시로 사용하능한 매매신청 구조체변수

        //// 얼마까지 살 수 있는 지를 확인하는 변수 모음
        public int curDeposit;  // 햔제 예수금
        public int ceilPrice;   // 총매수가능 상한가

        /// ///////////////////////////////////////


        public Form1()
        {
            InitializeComponent(); // c# 고유 고정메소드  

            MappingFileToStockCodes();
            SubscribeRealData();
            
            loginToolStripMenuItem.Click += ToolStripMenuItem_Click;
            checkMyAccountInfoButton.Click += Button_Click;

            /////////// Event Handler 
            axKHOpenAPI1.OnEventConnect += OnEventConnectHandler; // 로그인 event slot connect
            axKHOpenAPI1.OnReceiveTrData += OnReceiveTrDataHandler; // TR event slot connect
            axKHOpenAPI1.OnReceiveRealData += OnReceiveRealDataHandler; // 실시간 event slot connect
            axKHOpenAPI1.OnReceiveChejanData += OnReceiveChejanDataHandler; // 체결,접수,잔고 event slot connect
            ////////////////////////////////////////////////////////
            ///
           
        }



        /*
         * 주식종목들을 특정 txt파일에서 읽어
         * 코스닥, 코스피 변수에 string[] 형식으로 각각 저장
         * 코스닥, 코스피 종목갯수의 합만큼의 eachStockArray구조체 배열을 생성
         */
        private void MappingFileToStockCodes()
        {
            kosdaqCodes = System.IO.File.ReadAllLines(configurationPath + "today_kosdaq_stock_code.txt");
            kospiCodes = System.IO.File.ReadAllLines(configurationPath + "today_kospi_stock_code.txt");
            eachStockArray = new EachStock[kosdaqCodes.Length + kospiCodes.Length];
        }
        


        /*
         * string형  코스닥, 코스피 종목코드의 배열 string[n] 변수에서
         * 한 화면번호 당 (최대)100개씩 넣고 주식체결 fid를 넣고
         * 실시간 데이터 요청을 진행
         * 코스닥과 코스피 배열에서 100개가 안되는 나머지 종목들은 코스닥,코스피 각 다른 화면번호에 실시간 데이터 요청
         */
        private void SubscribeRealData()
        {
            
            int kosdaqIndex = 0;
            int kosdaqCodesLength = kosdaqCodes.Length;
            int kosdaqIterNum = kosdaqCodesLength / nSepPerScreen;
            int kosdaqRestNum = kosdaqCodesLength % nSepPerScreen;
            string strKosdaqCodeList;
            const string sFID = "228"; // 체결강도. 실시간 목록 FID들 중 겹치는게 가장 적은 FID

            // ------------------------------------------------------
            // 코스피 실시간 등록
            // ------------------------------------------------------
            // 100개 단위
            for (int i = 0; i< kosdaqIterNum; i++)
            {
                SetScreenNo();
                strKosdaqCodeList = ConvertStrCodeList(kosdaqCodes, kosdaqIndex, kosdaqIndex + nSepPerScreen);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKosdaqCodeList, sFID, "0");
                kosdaqIndex += nSepPerScreen;
            }
            // 나머지
            if (kosdaqRestNum > 0)
            {
                SetScreenNo();
                strKosdaqCodeList = ConvertStrCodeList(kosdaqCodes, kosdaqIndex, kosdaqIndex + kosdaqRestNum);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKosdaqCodeList, sFID, "0");
            }

            int kospiIndex = 0;
            int kospiCodesLength = kospiCodes.Length;
            int kospiIterNum = kospiCodesLength / nSepPerScreen;
            int kospiRestNum = kospiCodesLength % nSepPerScreen;
            string strKospiCodeList;

            // ------------------------------------------------------
            // 코스닥 실시간 등록
            // ------------------------------------------------------
            // 100개 단위
            for (int i = 0; i < kospiIterNum; i++)
            {
                SetScreenNo();
                strKospiCodeList = ConvertStrCodeList(kospiCodes, kospiIndex, kospiIndex + nSepPerScreen);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKospiCodeList, sFID, "0");
                kospiIndex += nSepPerScreen;
            }
            // 나머지
            if (kospiRestNum > 0)
            {
                SetScreenNo();
                strKospiCodeList = ConvertStrCodeList(kospiCodes, kospiIndex, kospiIndex + kospiRestNum);
                axKHOpenAPI1.SetRealReg(strScreenNum, strKospiCodeList, sFID, "0");
            }
        }



        /*
         * 매개변수 : 
         *  1.  string[] codes : 주식종목코드 배열
         *  2.  s : 배열의 시작 인덱스
         *  3.  e : 배열의 끝 인덱스 (포함 x)
         *  
         *  키움 실시간 신청메소드의 두번째 인자인 strCodeList는
         *  종목코드1;종목코드2;종목코드3;....;종목코드n(;마지막은 생략가능) 형식으로 넘겨줘야하기 때문에
         *  s부터 e -1 인덱스까지 string 변수에 추가하며 사이사이 ';'을 붙여준다
         *  
         *  실시간메소드에서 각 종목의 구조체를 사용하기 위해 초기화과정이 필요한데
         *  이 메소드에서 같이 진행해준다.
         */
        private string ConvertStrCodeList(string[] codes, int s, int e)
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
                curStock = eachStockArray[eachStockIdxArray[codeIdx]];
                curStock.screenNum = strScreenNum;
                curStock.code = codes[i];
                curStock.initMode = true;
                curStock.maxPower = -100.0;
                //////////////////////////////////////

                strKosdaqCodeList += codes[i];
                if (i < e - 1)
                    strKosdaqCodeList += ';';
            }
            return strKosdaqCodeList;
        }



   

        /*
         * StripMenuItem을 이용해
         * 메뉴 -> 로그인 버튼을 클릭하면 로그인 창이 뜸
         * 자동로그인 기능을 켜놨다면 로그인이 자동으로 진행됨.
         */
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(sender.Equals(loginToolStripMenuItem))
            {
                axKHOpenAPI1.CommConnect();
            }
        }



        /*
         * 초기화면번호는 1000번 
         * 메소드를 실행하면 strScreenNum 문자열변수에 string화된 화면번호를 저장 후
         * 기존화면번호를 한칸 올린다.
         * 화면번호는 4자리기 때문에 9999를 초과하면 1000번으로 되돌린다.(만약의 상황을 대비)
         */
        private void SetScreenNo()
        {
            if (screenNum > 9999)
                screenNum = 1000;

            strScreenNum = screenNum.ToString();
            screenNum++;
        }



        /*
         * 버튼 클릭 이벤트의 핸들러 메소드
         * 1. 예수금상세현황요청을 진행한다.
         */
        private void Button_Click(object sender, EventArgs e)
        {
            // 예수금상세현황요청
            if (sender.Equals(checkMyAccountInfoButton))
            {
                SetScreenNo();
                axKHOpenAPI1.SetInputValue("계좌번호", accountComboBox.Text);
                axKHOpenAPI1.SetInputValue("비밀번호", "");
                axKHOpenAPI1.SetInputValue("비밀번호입력매체구분", "00");
                axKHOpenAPI1.SetInputValue("조회구분", "2");
                axKHOpenAPI1.CommRqData("예수금상세현황요청", "opw00001", 0, strScreenNum);
            }
        }



        /*
         * TR 이벤트발생시 핸들러 메소드
         */
        private void OnReceiveTrDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            if (e.sRQName.Equals("예수금상세현황요청"))
            {
                int iMyDeposit = int.Parse(axKHOpenAPI1.GetCommData(e.sTrCode, e.sRecordName, 0, "예수금"));
                curDeposit = iMyDeposit;
                myDepositLabel.Text = iMyDeposit.ToString();
            }
        }



        /*
         * 로그인 이벤트발생시 핸들러 메소드
         */
        private void OnEventConnectHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0) // 로그인 성공
            {
                string myName = axKHOpenAPI1.GetLoginInfo("USER_NAME");
                string accList = axKHOpenAPI1.GetLoginInfo("ACCLIST"); // 로그인 사용자 계좌번호 리스트 요청
                string[] accountArray = accList.Split(';');
                foreach(string account in accountArray)
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


        /*
         * 실시간 이벤트발생시 핸들러메소드 
         */
        private void OnReceiveRealDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            if (e.sRealType.Equals("주식체결"))
            {

                string code = e.sRealKey;

                // -------------------------------------------------------------------------
                // 체결 요청
                // -------------------------------------------------------------------------
                // 매매컨트롤러 부분은 Async를 사용하려 했는데
                // 오버헤드가 클거같아서 실시간 부분에 끼워넣었다.
                if (tradeQueue.Count > 0)
                {
                    // 큐에서 하나 빼내 
                    
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
                curStock = eachStockArray[eachStockIdxArray[codeIdx]];

                if (!marketStart || curStock.passMode)
                    return;


                curStock.time = sharedTime = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 20))); // 현재시간
                curStock.fs = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 27))); // 최우선매도호가
                curStock.fb = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 28))); // 최우선매수호가
                curStock.tv = int.Parse(axKHOpenAPI1.GetCommRealData(code, 15));
                curStock.accumTradeQnt = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 13)));
                curStock.accumTradePrice = Math.Abs(int.Parse(axKHOpenAPI1.GetCommRealData(code, 14)));
                curStock.power = (curStock.fb - curStock.firstPrice) / curStock.firstPrice;
                curStock.idx++;

                if (!curStock.firstCheck)
                {
                    if (curStock.firstTime == 0) // 처음일때 시간설정
                    {
                        curStock.firstTime = curStock.time;
                        curStock.tenTime = AddTimeBySec(curStock.firstTime, 3600);
                        curStock.noonTime = 120000;
                    }

                    if (curStock.fs == 0)
                        return;
                    else
                    {
                        if (curStock.fb == 0)
                            curStock.fb = GetKosdaqGap(curStock.fs - 1);

                        curStock.firstPrice = curStock.fs;

                        if (curStock.fs < 1000)
                        {
                            // 실시간 제거대상
                            curStock.passMode = true;
                            axKHOpenAPI1.SetRealRemove(curStock.screenNum, curStock.code);
                        }
                        curStock.firstCheck = true;
                    }
                }

                // 이상 데이터 감지
                if ((curStock.fs - curStock.fb) / curStock.fb > 0.02)
                    return;


                ////// 전고점 부분 처리 /////////////////////
                if (curStock.crushCount <= 1 && SubTimeToTime(curStock.minTime, curStock.maxTime) >= 1 && SubTimeToTime(curStock.time, curStock.maxTime) >= 1 &&
                        (curStock.maxPower - curStock.minPower) > 0.01 && curStock.power > curStock.maxPower && curStock.time < 144000)
                {
                    curStock.crushCount++;
                    if (curStock.crushCount == 1 && curStock.power >= 0.1 && curStock.power <= 0.15)
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
                        curSlot.sOrgOrderNo = curStock.orgOrderNo;

                        tradeQueue.Enqueue(curSlot);

                    }
                }

                if (curStock.maxPower < curStock.power)
                {
                    curStock.maxPower = curStock.power;
                    curStock.maxTime = curStock.time;
                    curStock.minPower = curStock.power;
                    curStock.minTime = curStock.time;
                }

                if (curStock.minPower > curStock.power)
                {
                    curStock.minPower = curStock.power;
                    curStock.minTime = curStock.time;
                }
                //////////////////////////////////////////


                //////////// 바닥잡기 부분 ////////////////////
                if (curStock.time < curStock.noonTime && curStock.power < -0.17)
                {
                    if (curStock.idx > 1000)
                    {
                        // 구매해야 함
                    }
                    else
                    {
                        // 구매 안하고 종료
                    }
                }
            } // End ---- e.sRealType.Equals("주식체결")
            else if(e.sRealType.Equals("장시작시간"))
            {
                if (marketStart) // 실시간해지 신청을 했음에도 잔여시간동안 메시지가 올때를 대비한 조건문
                    return;

                string sGubun = axKHOpenAPI1.GetCommRealData(e.sRealKey, 215); // 장운영구분 0 :장시작전, 3 : 장중, 4 : 장종료
                if (sGubun.Equals('0'))
                {
                    testListView.Items.Add("장시작전");
                }
                else if (sGubun.Equals('3')) // 장중이라면 
                {
                    testListView.Items.Add("장중");
                    marketStart = true;
                }
            }
        }

        /*
         * 주식주문(접수, 체결, 잔고) 이벤트발생시 핸들러메소드
         */
        private void OnReceiveChejanDataHandler(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (e.sGubun.Equals('0')) // 접수와 체결 
            {
                string orderNum = axKHOpenAPI1.GetChejanData(9203); // 주문번호
                string code = axKHOpenAPI1.GetChejanData(9001); // 종목코드
                string orderTaskCategory = axKHOpenAPI1.GetChejanData(912); // 주문업무분류
                string orderStatus = axKHOpenAPI1.GetChejanData(913); // 주문상태(접수, 체결)
                string codeName = axKHOpenAPI1.GetChejanData(302);  // 종목명
                string orderType = axKHOpenAPI1.GetChejanData(905); // +매수, -매도
                string orderQuant = axKHOpenAPI1.GetChejanData(900); // 주문수량
                string orderPRice = axKHOpenAPI1.GetChejanData(901); // 주문가격
                string okTradePrice = axKHOpenAPI1.GetChejanData(910); // 체결가
                string okTradeQuant = axKHOpenAPI1.GetChejanData(911); // 체결량
                string noTradeQuant = axKHOpenAPI1.GetChejanData(902); // 미체결량
                string tradeTime = axKHOpenAPI1.GetChejanData(908); // 체결시간
                sharedTime = Math.Abs(int.Parse(tradeTime));

                int codeIdx = int.Parse(code);
                curStock =  eachStockArray[eachStockIdxArray[codeIdx]];
                bool sellMode = curStock.sellMode;

                if (orderStatus.Equals("체결"))
                {
                    if (noTradeQuant.Equals('0'))
                    {
                        lockDeposit = false;
                        curStock.orgStatus = false;
                        curStock.orgOrderNo = "";
                    }


                    if (sellMode == true) // 바로 매도 걸기
                    {

                    }
                    else // 실시간이나 잔고에서 가격 보고 결정하기
                    {

                    }
                }
                else if(orderStatus.Equals("접수"))
                {
                    curStock.orgStatus = true;
                    curStock.orgOrderNo = orderNum;
                }
            }
            else if(e.sGubun.Equals('1')) // 잔고
            {
                string code = axKHOpenAPI1.GetChejanData(9001); // 종목코드
                string codeName = axKHOpenAPI1.GetChejanData(302); // 종목코드
                string curPrice = axKHOpenAPI1.GetChejanData(10); // 현재가
                string curQuant = axKHOpenAPI1.GetChejanData(930); // 보유수량
                string tradePrice = axKHOpenAPI1.GetChejanData(931); // 매입단가
                string orderPossibleQuant = axKHOpenAPI1.GetChejanData(933); // 주문가능수량
            }
            else if(e.sGubun.Equals('4')) // 파생잔고
            {

            }
            else
            {
                Console.WriteLine("OnReceiveChejanDataHandler 해당 sGubun값이 존재하지 않음" + e.sGubun);
            }    
        }


    }
}
