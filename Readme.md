## Main Function: FinanceRatingProcessor

### Flow of the app:
- Assumed Company is created, with invoices and receivables imported as record.
- Create loan application to trigger the finance processor, which will return the following:
	1. the credit score of company based on its receivables turnover ratio 
	2. sets of healthy invoices, each with amount available to lend based on company credit score.

### Reference source:
1. https://www.investopedia.com/terms/r/receivableturnoverratio.asp
2. https://www.allianz-trade.com/en_SG/insights/risk-management/accounts-receivable-risk.html
3. https://www.investopedia.com/terms/a/average_collection_period.asp
4. https://www.investopedia.com/articles/basics/06/assetperformance.asp