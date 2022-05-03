from pykrx import stock
from datetime import datetime, timedelta


yesterday = datetime.now() #- timedelta(3)
dtype_yesterday = datetime.strftime(yesterday, '%Y%m%d')

kospi_df = stock.get_market_ticker_list(market= 'KOSPI')
stock_info_list = []

for idx, stk in enumerate(kospi_df):
    df = stock.get_market_ohlcv(dtype_yesterday,dtype_yesterday,stk)
    stock_info_list.append([stk, df['거래량'].iat[0]])

n_sep = 600
stock_info_list.sort(key= lambda x:-x[1])
sep_kospi_df = stock_info_list[:n_sep]


w = open('today_kospi_stock_code.txt','w')

for idx, stock in enumerate(sep_kospi_df):
    try:
        int(stock[0])
        w.write(stock[0])
        if idx < (n_sep -1):
            w.write('\n')
    except:
        pass

w.close()
