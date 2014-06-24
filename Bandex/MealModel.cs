using System;

namespace Bandex
{
    internal class MealModel
    {
        public enum MealTypes { Lunch, VegLunch, Dinner };

        private string _Content;
        private string _Title;
        private MealTypes _Type;

        public MealModel(string webPage, MealTypes type)
        {
            this._Type = type;
            this._Content = "";
            string[] cleanList = webPage.Split(new Char[] { '\n' });
            for (int i = 0; i < cleanList.Length; i++)
            {
                string partial = cleanList[i].Trim();
                if (partial.Length > 0)
                {
                    this._Content += partial + "\n";
                }
            }
            switch (type)
            {
                case MealTypes.Lunch:
                    this._Title = "Almoço";
                    break;

                case MealTypes.VegLunch:
                    this._Title = "Almoço Vegetariano";
                    break;

                default:
                    this._Title = "Jantar";
                    break;
            }
        }

        public MealModel(string titulo, string conteudo, MealTypes type = MealTypes.Lunch)
        {
            this._Title = titulo;
            this._Content = conteudo;
            this._Type = type;
        }

        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public MealTypes Type
        {
            get { return _Type; }
            set { Type = value; }
        }
    }
}