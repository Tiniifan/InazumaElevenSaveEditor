using System;
using System.Collections.Generic;

namespace InazumaElevenSaveEditor.Logic
{
    public class Player
    {
        public string Name;

        public UInt32 ID;

        public Element Element;

        public Position Position;

        public Gender Gender;

        public List<int> Stat;

        public int Freedom;

        public List<Move> Moves;

        public List<UInt32> UInt32Moves;

        public int Level;

        public bool Invoke;

        public bool Armed;

        public int Style;

        public List<int> InvestedPoint;

        public List<int> InvestedFreedom;

        public Avatar Avatar;

        public List<Equipment> Equipments;

        public long PositionInFile;

        public MixiMax MixiMax;

        public bool IsAura;

        public bool IsTrained;

        public Player()
        {

        }

        public Player(Player player)
        {
            Name = player.Name;
            Position = player.Position;
            Element = player.Element;
            Gender = player.Gender;
            UInt32Moves = player.UInt32Moves;
            Stat = player.Stat;
            Freedom = player.Freedom;
            InvestedFreedom = new List<int>(new int[8]);
        }

        public Player(string _Name, Position _Position, Element _Element, Gender _Gender, List<UInt32> _Moves, List<int> _Stat, int _Freedom)
        {
            Name = _Name;
            Position = _Position;
            Element = _Element;
            Gender = _Gender;
            UInt32Moves = _Moves;
            Stat = _Stat;
            Freedom = _Freedom;
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
