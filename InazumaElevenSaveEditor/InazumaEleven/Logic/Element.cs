namespace InazumaElevenSaveEditor.InazumaEleven.Logic
{
    public class Element
    {
        public string Name;

        public Element VeryEffective;

        public Element NotVeryEffective;

        public Element(string _Name)
        {
            Name = _Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public string Avantage()
        {
            if (this == Void()) return null;
            return VeryEffective.ToString();
        }

        public string Disadvantage()
        {
            if (this == Void()) return null;
            return NotVeryEffective.ToString();
        }

        public static Element Earth()
        {
            Element earth = new Element("Earth");
            earth.VeryEffective = Element.Fire();
            earth.NotVeryEffective = Element.Wind();
            return earth;
        }

        public static Element Fire()
        {
            Element fire = new Element("Fire");
            fire.VeryEffective = new Element("Wood");
            fire.NotVeryEffective = new Element("Earth");
            return fire;
        }

        public static Element Wind()
        {
            Element wind = new Element("Wind");
            wind.VeryEffective = new Element("Earth");
            wind.NotVeryEffective = new Element("Wood");
            return wind;
        }

        public static Element Wood()
        {
            Element wood = new Element("Wood");
            wood.VeryEffective = new Element("Wind");
            wood.NotVeryEffective = new Element("Fire");
            return wood;
        }

        public static Element Void()
        {
            Element voidElement = new Element("Void");
            voidElement.VeryEffective = null;
            voidElement.NotVeryEffective = null;
            return voidElement;
        }
    }
}
