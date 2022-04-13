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
            /////////////////////////
        }
    }
}
