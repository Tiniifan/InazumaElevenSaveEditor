using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class Move
    {
        public string Name;

        public Element Element;

        public Position Position;

        public int Power;

        public int TP;

        public int Difficulty;

        public int Damage;

        public int EvolutionCount;

        public EvolutionSpeed EvolutionSpeed;

        public List<int> PowerLevel = new List<int>();

        public int Level;

        public bool Unlock;

        public Move()
        {
        }

        public Move(string _Name, string _Element, string _Position, int _Power, int _Tp, int _Difficulty, int _Damage, int _EvolutionCount, string _EvolutionSpeed, string _Game)
        {
            Name = _Name;
            switch (_Element)
            {
                case "Earth":
                    Element = Element.Earth();
                    break;
                case "Fire":
                    Element = Element.Fire();
                    break;
                case "Wind":
                    Element = Element.Wind();
                    break;
                case "Wood":
                    Element = Element.Wood();
                    break;
                case "Void":
                    Element = Element.Void();
                    break;
                default:
                    Element = null;
                    break;
            }
            switch (_Position)
            {
                case "Shoot":
                    Position = Position.Shoot();
                    break;
                case "Dribble":
                    Position = Position.Dribble();
                    break;
                case "Catch":
                    Position = Position.Block();
                    break;
                case "Block":
                    Position = Position.Catch();
                    break;
                default:
                    Position = null;
                    break;
            }
            switch (_EvolutionSpeed)
            {
                case "Slow":
                    EvolutionSpeed = EvolutionSpeed.Slow(_EvolutionCount, _Game);
                    break;
                case "Medium":
                    EvolutionSpeed = EvolutionSpeed.Medium(_EvolutionCount, _Game);
                    break;
                case "Fast":
                    EvolutionSpeed = EvolutionSpeed.Fast(_EvolutionCount, _Game);
                    break;
                case "Turbo":
                    EvolutionSpeed = EvolutionSpeed.Turbo();
                    break;
                default:
                    EvolutionSpeed = null;
                    break;
            }
            Power = _Power;
            TP = _Tp;
            Difficulty = _Difficulty;
            Damage = _Damage;
            EvolutionCount = _EvolutionCount;
            Level = 1;

            if (this.EvolutionSpeed != null)
            {
                PowerLevel.Add(_Power);

                if (this.EvolutionSpeed.PowerLevel.Count != 0)
                {
                    for (int i = 0; i < EvolutionCount - 1; i++)
                    {
                        PowerLevel.Add(_Power + EvolutionSpeed.PowerLevel[i]);
                    }
                }
                else
                {
                    if (_EvolutionCount < 6)
                    {
                        for (int i = 0; i < EvolutionCount - 1; i++)
                        {
                            PowerLevel.Add(PowerLevel[i] + (10 / PowerLevel[i] * 100));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            PowerLevel.Add(PowerLevel[i] + (10 / PowerLevel[i] * 100));
                        }
                        // Missing Ultimate Evolution
                    }
                }

                EvolutionSpeed.TimeLevel.Insert(0, 0);
            }
        }

        public Move(string _Name, Element _Element, Position _Position, int _Power, int _Tp, int _Difficulty, int _Damage, int _EvolutionCount, EvolutionSpeed _EvolutionSpeed)
        {
            Name = _Name;
            Element = _Element;
            Position = _Position;
            Power = _Power;
            TP = _Tp;
            Difficulty = _Difficulty;
            Damage = _Damage;
            EvolutionCount = _EvolutionCount;
            EvolutionSpeed = _EvolutionSpeed;
            Level = 1;

            if (this.EvolutionSpeed != null)
            {
                PowerLevel.Add(_Power);

                if (this.EvolutionSpeed.PowerLevel.Count != 0)
                {
                    for (int i = 0; i < EvolutionCount - 1; i++)
                    {
                        PowerLevel.Add(_Power + EvolutionSpeed.PowerLevel[i]);
                    }
                }
                else
                {
                    if (_EvolutionCount < 6)
                    {
                        for (int i = 0; i < EvolutionCount - 1; i++)
                        {
                            PowerLevel.Add(PowerLevel[i] + (10 / PowerLevel[i] * 100));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            PowerLevel.Add(PowerLevel[i] + (10 / PowerLevel[i] * 100));
                        }
                        // Missing Ultimate Evolution
                    }
                }

                EvolutionSpeed.TimeLevel.Insert(0, 0);
            }
        }

        public int? PowerAtLevel(int level)
        {
            if (level > PowerLevel.Count)
            {
                return null;
            }
            else if (level == 0 | level == 1)
            {
                return PowerLevel[0];
            }
            else
            {
                return PowerLevel[level - 1];
            }
        }
    }

    public class EvolutionSpeed
    {
        public string Name;

        public List<int> PowerLevel = new List<int>();

        public List<int> TimeLevel = new List<int>();

        public EvolutionSpeed(string _Name, List<int> _PowerUp, List<int> _TimeLevel)
        {
            Name = _Name;
            PowerLevel = _PowerUp;
            TimeLevel = _TimeLevel;
        }

        public static EvolutionSpeed Slow(int evolutionCount, string game)
        {
            if (evolutionCount == 3)
            {
                return new EvolutionSpeed("Slow", new List<int> { 5, 12 }, new List<int> { 15, 33 });
            }
            else if (evolutionCount == 4)
            {
                return new EvolutionSpeed("Slow", new List<int> { }, new List<int> { 20, 30, 35 });
            }
            else if (evolutionCount == 5 & game == "ie")
            {
                return new EvolutionSpeed("Slow", new List<int> { 4, 10, 14, 18 }, new List<int> { 20, 45, 70, 100 });
            }
            else if (evolutionCount == 5 & game == "go")
            {
                return new EvolutionSpeed("Slow", new List<int> { }, new List<int> { 20, 30, 35, 40 });
            }
            else // evolutionCount == 6
            {
                return new EvolutionSpeed("Slow", new List<int> { }, new List<int> { 15, 20, 20, 25 });
            }
        }

        public static EvolutionSpeed Medium(int evolutionCount, string game)
        {
            if (evolutionCount == 3)
            {
                return new EvolutionSpeed("Medium", new List<int> { 3, 10 }, new List<int> { 9, 27 });
            }
            else if (evolutionCount == 4)
            {
                return new EvolutionSpeed("Medium", new List<int> { }, new List<int> { 15, 20, 30 });
            }
            else if (evolutionCount == 5 & game == "ie")
            {
                return new EvolutionSpeed("Medium", new List<int> { 3, 8, 12, 16 }, new List<int> { 20, 40, 60, 90 });
            }
            else if (evolutionCount == 5 & game == "go")
            {
                return new EvolutionSpeed("Medium", new List<int> { }, new List<int> { 15, 20, 30, 40 });
            }
            else // evolutionCount == 6
            {
                return new EvolutionSpeed("Medium", new List<int> { }, new List<int> { 10, 15, 20, 25 });
            }
        }

        public static EvolutionSpeed Fast(int evolutionCount, string game)
        {
            if (evolutionCount == 3)
            {
                return new EvolutionSpeed("Fast", new List<int> { 2, 8 }, new List<int> { 6, 21 });
            }
            else if (evolutionCount == 4)
            {
                return new EvolutionSpeed("Fast", new List<int> { }, new List<int> { 10, 15, 25 });
            }
            else if (evolutionCount == 5 & game == "ie")
            {
                return new EvolutionSpeed("Fast", new List<int> { 2, 6, 10, 14 }, new List<int> { 15, 35, 55, 80 });
            }
            else if (evolutionCount == 5 & game == "go")
            {
                return new EvolutionSpeed("Fast", new List<int> { }, new List<int> { 10, 15, 25, 40 });
            }
            else // evolutionCount == 6
            {
                return new EvolutionSpeed("Fast", new List<int> { }, new List<int> { 10, 10, 15, 20 });
            }
        }

        public static EvolutionSpeed Turbo()
        {
            return new EvolutionSpeed("Turbo", new List<int> { }, new List<int> { 5, 10, 10, 15 });
        }
    }
}
