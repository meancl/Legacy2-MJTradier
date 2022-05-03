namespace MJTradier
{
    public partial class Form1
    {
        // ============================================
        // 각 종목이 가지는 개인 구조체
        // ============================================
        public struct EachStock
        {
            // ----------------------------------
            // 기본정보 변수
            // ----------------------------------
            public string sRealScreenNum;
            public string sCode;
            public int nMarketGubun; // 0이면 코스닥, 1이면 코스피

            // ----------------------------------
            // 초기 변수
            // ----------------------------------
            public bool isFirstCheck;
            public int nFirstTime;
            public int nFirstPrice;
            public int nTenTime;

            // ----------------------------------
            // 실시간체결 변수
            // ----------------------------------
            public int nFs;
            public int nFb;
            public int nIdx;
            public double fPower;

            // ----------------------------------
            // 전고점 변수
            // ----------------------------------
            public int nMinTime;
            public int nMaxTime;
            public double fMaxPower;
            public double fMinPower;

            // ----------------------------------
            // 바닥잡기 변수
            // ----------------------------------
            public int nNoonTime;   // 11시로 하느냐, 12시로 하느냐

            // ----------------------------------
            // 매매관련 변수
            // ----------------------------------
            public int nCurLimitPrice; // 지정가가 estimatedPrice를 초과하는 미체결 수량이 남았다면 처분하기 위한 변수
            public int nCurRqTime; // 매수주문했을때의 시간
            public bool isOrderStatus; // 현재 매매중인 지 확인하는 변수;
            public string sCurOrgOrderId; // 원주문번호   default:""
            public int nBuyReqCnt; // 현재 종목의 매수신청카운트
            public int nSellReqCnt; // 현재 종목의 매도신청카운트 
            public bool isCancelMode; // 현재 매수에서 매수취소가 나왔으면 더이상의 현재의 거래에서 매수취소요청을 금지하기 위한 변수
            public bool isCancelComplete; // 매수취소가 성공한 경우를 판별하는 변수
            public int nHoldingsCnt; // 보유종목수
            public double fTargetPercent; // 익절 퍼센트
            public double fBottomPercent; // 손절 퍼센트
            
        }



        // ============================================
        // 매매요청 큐에 저장하기 위한 구조체변수
        // ============================================
        public struct TradeSlot
        {
            public int nRqTime; // 주문요청시간
            public double fTargetPercent; // 익절 퍼센트 
            public double fBottomPercent; // 손절 퍼센트 
            public int nEachStockIdx; // 개인구조체인덱스
            public int nBuySlotIdx; // 구매열람인덱스 , 매도요청이 실패하면 해당인덱스를 통해 다시 요청할 수 있게 하기 위한 변수
            // ----------------------------------
            // SendOrder 인자들
            // ----------------------------------
            public string sRQName; // 사용자 구분명
            public string sScreenNo; // 화면번호
            public string sAccNo; // 계좌번호 10자리
            public int nOrderType; // 주문유형 1:신규매수, 2:신규매도 3:매수취소, 4:매도취소, 5:매수정정, 6:매도정정
            public string sCode; // 종목코드(6자리)
            public int nQty; // 주문수량
            public int nOrderPrice; // 주문가격
            public string sHogaGb; // 거래구분 (00:지정가, 03:시장가, ...)
            public string sOrgOrderId;  // 원주문번호. 신규주문에는 공백 입력, 정정/취소시 입력합니다.
        }

        // ============================================
        // 현재보유종목 열람용 구조체변수
        // ============================================
        public struct Holdings
        {
            public string sCode;
            public string sCodeName;
            public double fYield;
            public int nHoldingQty;
            public int nBuyedPrice;
            public int nCurPrice;
            public int nTotalPL;
            public int nNumPossibleToSell;
        }

        // ============================================
        // 개인구조체 구매횟수 구조체
        // ============================================
        public struct BuySlot //TODAY
        {
            public int nBuyPrice; // 얼마에 구매했어
            public int nBuyVolume; // 얼마나 구매했어
            public double fTargetPer; // 얼마에 익절할거야
            public double fBottomPer; // 얼마에 손절할거야
            public bool isSelled; // 전부 팔렸어
            public bool isAllBuyed; // 전부 사졌어
        }

    }
}
