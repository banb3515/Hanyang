namespace Hanyang.BindingData
{
    public class LunchMenu
    {
        public string Symbol { get; set; } // 글머리 기호 색상 (HEX)
        public string Food { get; set; } // 음식 이름

        public LunchMenu(string symbol, string food)
        {
            Symbol = symbol;
            Food = food;
        }
    }
}
