using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class Player
    {
        public string Name;

        public Element Element;

        public Position Position;

        public Gender Gender;

        public List<int> Stat;

        public List<Move> Techniques;

        public int Level;

        public bool Invoke;

        public bool Armed;

        public int Style;

        public List<int> InvestedPoint;

        public List<int> InvestedFreedom;

        public Avatar Avatar;

        public List<Equipment> Equipments;

        public string IndexSorted;

        public int PositionInFile;

        public MixiMax MixiMax;

        public bool IsAura;

        public bool IsTrained;

        public Player()
        {

        }

        public Player(string _Name, string _Position, string _Element, string _Gender, List<int> _Stat, List<Move> _Techniques)
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
                    Element = Element.Earth();
                    break;
            }
            switch (_Position)
            {
                case "Forward":
                    Position = Position.Forward();
                    break;
                case "Midfielder":
                    Position = Position.Midfielder();
                    break;
                case "Defender":
                    Position = Position.Defender();
                    break;
                case "Goalkeeper":
                    Position = Position.Goalkeeper();
                    break;
                default:
                    Position = Position.None();
                    break;
            }
            switch (_Gender)
            {
                case "Boy":
                    Gender = Gender.Boy();
                    break;
                case "Girl":
                    Gender = Gender.Girl();
                    break;
                case "Unknow":
                    Gender = Gender.Unknow();
                    break;
                default:
                    Gender = Gender.Unknow();
                    break;
            }
            Stat = _Stat;
            Techniques = _Techniques;
            InvestedFreedom = new List<int>(new int[8]);
        }

        public void Training(int _StatNumber, int _Add, int _Remove)
        {
            if (Position.Name == "Forward")
            {
                switch (_StatNumber)
                {
                    case 0: // Kick
                        if (IsTrained == false)
                        {
                            InvestedPoint[0] = _Add;
                        }
                        else
                        {
                            InvestedPoint[0] = _Add;
                            InvestedPoint[2] = _Remove;

                        }
                        break;
                    case 1: // Dribble
                        if (IsTrained == false)
                        {
                            InvestedPoint[1] = _Add;
                        }
                        else
                        {
                            InvestedPoint[1] = _Add;
                            InvestedPoint[3] = _Remove;
                        }
                        break;
                    case 2: // Technique
                        if (IsTrained == false)
                        {
                            InvestedPoint[2] = _Add;
                        }
                        else
                        {
                            InvestedPoint[2] = _Add;
                            InvestedPoint[0] = _Remove;
                        }
                        break;
                    case 3: // Block
                        if (IsTrained == false)
                        {
                            InvestedPoint[3] = _Add;
                        }
                        else
                        {
                            InvestedPoint[3] = _Add;
                            InvestedPoint[1] = _Remove;
                        }
                        break;
                    case 4: // Speed
                        if (IsTrained == false)
                        {
                            InvestedPoint[4] = _Add;
                        }
                        else
                        {
                            InvestedPoint[4] = _Add;
                            InvestedPoint[5] = _Remove;
                        }
                        break;
                    case 5: // Stamina
                        if (IsTrained == false)
                        {
                            InvestedPoint[5] = _Add;
                        }
                        else
                        {
                            InvestedPoint[5] = _Add;
                            InvestedPoint[4] = _Remove;
                        }
                        break;
                    case 6: // Catch
                        if (IsTrained == false)
                        {
                            InvestedPoint[6] = _Add;
                        }
                        else
                        {
                            InvestedPoint[6] = _Add;
                            InvestedPoint[7] = _Remove;
                        }
                        break;
                    case 7: // Luck
                        if (IsTrained == false)
                        {
                            InvestedPoint[7] = _Add;
                        }
                        else
                        {
                            InvestedPoint[7] = _Add;
                            InvestedPoint[6] = _Remove;
                        }
                        break;
                }
            }
            else if (Position.Name == "Midfielder")
            {
                switch (_StatNumber)
                {
                    case 0: // Kick
                        if (IsTrained == false)
                        {
                            InvestedPoint[0] = _Add;
                        }
                        else
                        {
                            InvestedPoint[0] = _Add;
                            InvestedPoint[6] = _Remove;

                        }
                        break;
                    case 1: // Dribble
                        if (IsTrained == false)
                        {
                            InvestedPoint[1] = _Add;
                        }
                        else
                        {
                            InvestedPoint[1] = _Add;
                            InvestedPoint[2] = _Remove;
                        }
                        break;
                    case 2: // Technique
                        if (IsTrained == false)
                        {
                            InvestedPoint[2] = _Add;
                        }
                        else
                        {
                            InvestedPoint[2] = _Add;
                            InvestedPoint[1] = _Remove;
                        }
                        break;
                    case 3: // Block
                        if (IsTrained == false)
                        {
                            InvestedPoint[3] = _Add;
                        }
                        else
                        {
                            InvestedPoint[3] = _Add;
                            InvestedPoint[7] = _Remove;
                        }
                        break;
                    case 4: // Speed
                        if (IsTrained == false)
                        {
                            InvestedPoint[4] = _Add;
                        }
                        else
                        {
                            InvestedPoint[4] = _Add;
                            InvestedPoint[5] = _Remove;
                        }
                        break;
                    case 5: // Stamina
                        if (IsTrained == false)
                        {
                            InvestedPoint[5] = _Add;
                        }
                        else
                        {
                            InvestedPoint[5] = _Add;
                            InvestedPoint[4] = _Remove;
                        }
                        break;
                    case 6: // Catch
                        if (IsTrained == false)
                        {
                            InvestedPoint[6] = _Add;
                        }
                        else
                        {
                            InvestedPoint[6] = _Add;
                            InvestedPoint[0] = _Remove;
                        }
                        break;
                    case 7: // Luck
                        if (IsTrained == false)
                        {
                            InvestedPoint[7] = _Add;
                        }
                        else
                        {
                            InvestedPoint[7] = _Add;
                            InvestedPoint[3] = _Remove;
                        }
                        break;
                }
            }
            else if (Position.Name == "Defender")
            {
                switch (_StatNumber)
                {
                    case 0: // Kick
                        if (IsTrained == false)
                        {
                            InvestedPoint[0] = _Add;
                        }
                        else
                        {
                            InvestedPoint[0] = _Add;
                            InvestedPoint[6] = _Remove;

                        }
                        break;
                    case 1: // Dribble
                        if (IsTrained == false)
                        {
                            InvestedPoint[1] = _Add;
                        }
                        else
                        {
                            InvestedPoint[1] = _Add;
                            InvestedPoint[7] = _Remove;
                        }
                        break;
                    case 2: // Technique
                        if (IsTrained == false)
                        {
                            InvestedPoint[2] = _Add;
                        }
                        else
                        {
                            InvestedPoint[2] = _Add;
                            InvestedPoint[3] = _Remove;
                        }
                        break;
                    case 3: // Block
                        if (IsTrained == false)
                        {
                            InvestedPoint[3] = _Add;
                        }
                        else
                        {
                            InvestedPoint[3] = _Add;
                            InvestedPoint[2] = _Remove;
                        }
                        break;
                    case 4: // Speed
                        if (IsTrained == false)
                        {
                            InvestedPoint[4] = _Add;
                        }
                        else
                        {
                            InvestedPoint[4] = _Add;
                            InvestedPoint[5] = _Remove;
                        }
                        break;
                    case 5: // Stamina
                        if (IsTrained == false)
                        {
                            InvestedPoint[5] = _Add;
                        }
                        else
                        {
                            InvestedPoint[5] = _Add;
                            InvestedPoint[4] = _Remove;
                        }
                        break;
                    case 6: // Catch
                        if (IsTrained == false)
                        {
                            InvestedPoint[6] = _Add;
                        }
                        else
                        {
                            InvestedPoint[6] = _Add;
                            InvestedPoint[0] = _Remove;
                        }
                        break;
                    case 7: // Luck
                        if (IsTrained == false)
                        {
                            InvestedPoint[7] = _Add;
                        }
                        else
                        {
                            InvestedPoint[7] = _Add;
                            InvestedPoint[1] = _Remove;
                        }
                        break;
                }
            }
            else if (Position.Name == "Goalkeeper")
            {
                switch (_StatNumber)
                {
                    case 0: // Kick
                        if (IsTrained == false)
                        {
                            InvestedPoint[0] = _Add;
                        }
                        else
                        {
                            InvestedPoint[0] = _Add;
                            InvestedPoint[7] = _Remove;

                        }
                        break;
                    case 1: // Dribble
                        if (IsTrained == false)
                        {
                            InvestedPoint[1] = _Add;
                        }
                        else
                        {
                            InvestedPoint[1] = _Add;
                            InvestedPoint[3] = _Remove;
                        }
                        break;
                    case 2: // Technique
                        if (IsTrained == false)
                        {
                            InvestedPoint[2] = _Add;
                        }
                        else
                        {
                            InvestedPoint[2] = _Add;
                            InvestedPoint[6] = _Remove;
                        }
                        break;
                    case 3: // Block
                        if (IsTrained == false)
                        {
                            InvestedPoint[3] = _Add;
                        }
                        else
                        {
                            InvestedPoint[3] = _Add;
                            InvestedPoint[1] = _Remove;
                        }
                        break;
                    case 4: // Speed
                        if (IsTrained == false)
                        {
                            InvestedPoint[4] = _Add;
                        }
                        else
                        {
                            InvestedPoint[4] = _Add;
                            InvestedPoint[5] = _Remove;
                        }
                        break;
                    case 5: // Stamina
                        if (IsTrained == false)
                        {
                            InvestedPoint[5] = _Add;
                        }
                        else
                        {
                            InvestedPoint[5] = _Add;
                            InvestedPoint[4] = _Remove;
                        }
                        break;
                    case 6: // Catch
                        if (IsTrained == false)
                        {
                            InvestedPoint[6] = _Add;
                        }
                        else
                        {
                            InvestedPoint[6] = _Add;
                            InvestedPoint[2] = _Remove;
                        }
                        break;
                    case 7: // Luck
                        if (IsTrained == false)
                        {
                            InvestedPoint[7] = _Add;
                        }
                        else
                        {
                            InvestedPoint[7] = _Add;
                            InvestedPoint[0] = _Remove;
                        }
                        break;
                }
            }
            else
            {
                switch (_StatNumber)
                {
                    case 0: // Kick
                        if (IsTrained == false)
                        {
                            InvestedPoint[0] = _Add;
                        }
                        else
                        {
                            InvestedPoint[0] = _Add;
                            InvestedPoint[7] = _Remove;

                        }
                        break;
                    case 1: // Dribble
                        if (IsTrained == false)
                        {
                            InvestedPoint[1] = _Add;
                        }
                        else
                        {
                            InvestedPoint[1] = _Add;
                            InvestedPoint[3] = _Remove;
                        }
                        break;
                    case 2: // Technique
                        if (IsTrained == false)
                        {
                            InvestedPoint[2] = _Add;
                        }
                        else
                        {
                            InvestedPoint[2] = _Add;
                            InvestedPoint[6] = _Remove;
                        }
                        break;
                    case 3: // Block
                        if (IsTrained == false)
                        {
                            InvestedPoint[3] = _Add;
                        }
                        else
                        {
                            InvestedPoint[3] = _Add;
                            InvestedPoint[1] = _Remove;
                        }
                        break;
                    case 4: // Speed
                        if (IsTrained == false)
                        {
                            InvestedPoint[4] = _Add;
                        }
                        else
                        {
                            InvestedPoint[4] = _Add;
                            InvestedPoint[5] = _Remove;
                        }
                        break;
                    case 5: // Stamina
                        if (IsTrained == false)
                        {
                            InvestedPoint[5] = _Add;
                        }
                        else
                        {
                            InvestedPoint[5] = _Add;
                            InvestedPoint[4] = _Remove;
                        }
                        break;
                    case 6: // Catch
                        if (IsTrained == false)
                        {
                            InvestedPoint[6] = _Add;
                        }
                        else
                        {
                            InvestedPoint[6] = _Add;
                            InvestedPoint[2] = _Remove;
                        }
                        break;
                    case 7: // Luck
                        if (IsTrained == false)
                        {
                            InvestedPoint[7] = _Add;
                        }
                        else
                        {
                            InvestedPoint[7] = _Add;
                            InvestedPoint[0] = _Remove;
                        }
                        break;
                }
            }
        }

        public int FallStat(int up_StatNumber)
        {
            int down_StatNumber = 0;

            if (Position.Name == "Forward")
            {
                switch (up_StatNumber)
                {
                    case 0: // Kick
                        return 2;
                    case 1: // Dribble
                        return 3;
                    case 2: // Technique
                        return 0;
                    case 3: // Block
                        return 1;
                    case 4: // Speed
                        return 5;
                    case 5: // Stamina
                        return 4;
                    case 6: // Catch
                        return 7;
                    case 7: // Luck
                        return 6;
                }
            }
            else if (Position.Name == "Midfielder")
            {
                switch (up_StatNumber)
                {
                    case 0: // Kick
                        return 6;
                    case 1: // Dribble
                        return 2;
                    case 2: // Technique
                        return 1;
                    case 3: // Block
                        return 7;
                    case 4: // Speed
                        return 5;
                    case 5: // Stamina
                        return 4;
                    case 6: // Catch
                        return 0;
                    case 7: // Luck
                        return 3;
                }
            }
            else if (Position.Name == "Defender")
            {
                switch (up_StatNumber)
                {
                    case 0: // Kick
                        return 6;
                    case 1: // Dribble
                        return 7;
                    case 2: // Technique
                        return 3;
                    case 3: // Block
                        return 2;
                    case 4: // Speed
                        return 5;
                    case 5: // Stamina
                        return 4;
                    case 6: // Catch
                        return 0;
                    case 7: // Luck
                        return 1;
                }
            }
            else if (Position.Name == "Goalkeeper")
            {
                switch (up_StatNumber)
                {
                    case 0: // Kick
                        return 7;
                    case 1: // Dribble
                        return 3;
                    case 2: // Technique
                        return 6;
                    case 3: // Block
                        return 1;
                    case 4: // Speed
                        return 5;
                    case 5: // Stamina
                        return 4;
                    case 6: // Catch
                        return 2;
                    case 7: // Luck
                        return 0;
                }
            }
            else
            {
                switch (up_StatNumber)
                {
                    case 0: // Kick
                        return 7;
                    case 1: // Dribble
                        return 3;
                    case 2: // Technique
                        return 6;
                    case 3: // Block
                        return 1;
                    case 4: // Speed
                        return 5;
                    case 5: // Stamina
                        return 4;
                    case 6: // Catch
                        return 2;
                    case 7: // Luck
                        return 0;
                }
            }

            return down_StatNumber;
        }
    }
}
