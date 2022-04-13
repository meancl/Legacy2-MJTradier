namespace MJTradier
{
    public partial class Form1
    {
        public struct EachStock
        {
            public string screenNum;
            public string code;
            public int idx;
            public double power;

            public int firstTime;
            public int firstPrice;
            public bool firstCheck;
            public int tenTime;

            /// 거래정도 측정변수
            public long accumTradeQnt;
            public long accumTradePrice;
            /////////////////////

            /// 실시간체결 변수
            public int time;
            public int fs;
            public int fb;
            public int tv;
            //////////////////

            /// 전고점 변수
            public int minTime;
            public int maxTime;
            public double maxPower;
            public double minPower;
            public int crushCount;
            ///////////////////

            /// 바닥잡기 변수
            public int noonTime;   // 11시로 하느냐, 12시로 하느냐
            /////////////////////

            /// 플래그 변수
            public bool initMode;  // 처음상태확인 플래그 : true면 처음, false면 사용중
            public bool sellMode;  // 매도전략 플래그 : true면 바로 매도거는 전략, false면 가격확인 실시간 매도하는 전략
            public bool passMode;  // 이제 실시간을 더 안들여봐도 되는 종목인 경우 true로 설정
            ///////////////////

            /// 매도관련 변수
            public double targetPercent; // 익절 퍼센트
            public double bottomPercent; // 손절 퍼센트
            /////////////////////

            /// 매수관련 변수
            public double buyPower;
            public int buyTime;
            public int buyPrice;
            public int buyCount;
            public bool orgStatus;
            public string orgOrderNo;
            /////////////////////////
        }

        public struct TradeSlot
        {
            public int nRqTime; // 주문요청시간

            ////////////// SendOrder 인자들
            public string sRQName; // 사용자 구분명
            public string sScreenNo; // 화면번호
            public string sAccNo; // 계좌번호 10자리
            public int nOrderType; // 주문유형 1:신규매수, 2:신규매도 3:매수취소, 4:매도취소, 5:매수정정, 6:매도정정
            public string sCode; // 종목코드(6자리)
            public int nQty; // 주문수량
            public int nPrice; // 주문가격
            public string sHogaGb; // 거래구분 (00:지정가, 03:시장가, ...)
            public string sOrgOrderNo;  // 원주문번호. 신규주문에는 공백 입력, 정정/취소시 입력합니다.
            /////////////////////////////////////////////////////
        }

    }
}
