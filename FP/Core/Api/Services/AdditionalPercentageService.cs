namespace FP.Core.Api.Services;

public class AdditionalPercentageService
{
	public static decimal DefineAddPercent(int days)
	{
		decimal additionalPercentage = 0;
		switch (days)
		{
			case > 10 and <= 20:
				additionalPercentage = 0.1m;
				break;
			case > 20 and <= 30:
				additionalPercentage = 0.15m;
				break;
			case > 30 and <= 50:
				additionalPercentage = 0.2m;
				break;
			case > 50 and <= 100:
				additionalPercentage = 0.25m;
				break;
			case > 100 and <= 200:
				additionalPercentage = 0.3m;
				break;
			case > 200 and <= 400:
				additionalPercentage = 0.35m;
				break;
			case > 400 and <= 800:
				additionalPercentage = 0.4m;
				break;
			case > 800:
				additionalPercentage = 0.8m;
				break;
		}
		return additionalPercentage;
	}
}

