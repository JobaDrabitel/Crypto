namespace FP.Core.Api.Services;

public class RangSystem
{
    public static decimal SumToNextRang(int rang)
    {
        return rang switch
        {
            0 => 100m,
            1 => 1000m,
            2 => 2000m,
            3 => 5000m,
            4 => 30000m,
            5 => 100000m,
            6 => 500000m,
            7 => 1000000m,
            8 => 2000000m,
            9 => 5000000m,
            10 => 10000000m,
            11 => 30000000m,
            12 => 80000000m,
            _ => 0
        };
    }
}