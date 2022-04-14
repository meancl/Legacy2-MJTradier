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
            public string screenNum;
            public string code;
            public int marketGubun; // 1이면 코스닥, 2이면 코스피

            // ----------------------------------
            // 초기 변수
            // ----------------------------------
            public bool firstCheck;
            public int firstTime;
            public int firstPrice;
            public int tenTime;

            // ----------------------------------
            // 실시간체결 변수
            // ----------------------------------
            public int time;
            public int fs;
            public int fb;
            public int tv;
            public long accumTradeQnt;
            public long accumTradePrice;
            public int idx;
            public double power;

            // ----------------------------------
            // 전고점 변수
            // ----------------------------------
            public int minTime;
            public int maxTime;
            public double maxPower;
            public double minPower;
            public int crushCount;

            // ----------------------------------
            // 바닥잡기 변수
            // ----------------------------------
            public int noonTime;   // 11시로 하느냐, 12시로 하느냐

            // ----------------------------------
            // 플래그 변수
            // ----------------------------------
            public bool initMode;  // (삭제예정)처음상태확인 플래그 : true면 처음, false면 사용중
            public bool passMode;  // 이제 실시간을 더 안들여봐도 되는 종목인 경우 true로 설정

            // ----------------------------------
            // 매매관련 변수
            // ----------------------------------
            public int nOrderType; // 1:신규매수 2:신규매도 3:매수취소 4:매도취소 5:매수정정 6:매도정정
            public int nOrderTime;
            public int nOrderPrice;
            public bool orgStatus; // 현재 매매중인 지 확인하는 변수
            public string orgOrderNo; // 원주문번호   default:""

            // ----------------------------------
            // 매도관련 변수
            // ----------------------------------
            public double targetPercent; // 익절 퍼센트
            public double bottomPercent; // 손절 퍼센트
            public bool bTradeEnds; // 매매가 완료되었는 지 확인하는 변수
            public int nBuyedPrice; // 전량체결됐을때 매입단가
            public int nBuyingPrice; // 미체결된 거래가 남아있을 때 매입단가
            public int sellMode;  // 매도전략 플래그 : 1이면 지정가매도, 2이면 시장가매도
            
            // ----------------------------------
            // 매수관련 변수
            // ----------------------------------
            
            
        }

        // ============================================
        // 매매요청 큐에 저장하기 위한 구조체변수
        // ============================================
        public struct TradeSlot
        {
            public int nRqTime; // 주문요청시간

            // ----------------------------------
            // SendOrder 인자들
            // ----------------------------------
            public string sRQName; // 사용자 구분명
            public string sScreenNo; // 화면번호
            public string sAccNo; // 계좌번호 10자리
            public int nOrderType; // 주문유형 1:신규매수, 2:신규매도 3:매수취소, 4:매도취소, 5:매수정정, 6:매도정정
            public string sCode; // 종목코드(6자리)
            public int nQty; // 주문수량
            public int nPrice; // 주문가격
            public string sHogaGb; // 거래구분 (00:지정가, 03:시장가, ...)
            public string sOrgOrderNo;  // 원주문번호. 신규주문에는 공백 입력, 정정/취소시 입력합니다.
            
        }

        // ============================================
        // 현재보유종목 열람용 구조체변수
        // ============================================
        public struct Holdings
        {
            public string code;
            public string codeName;
            public double yield;
            public int holdingQty;
            public int buyedPrice;
        }

    }
}
