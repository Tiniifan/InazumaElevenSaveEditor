# Inazuma Eleven Go Chrono Stone Save Explained

Important Address
Offset | Name
--- | ---
0x3C | Save Name 
0x64 | Team Name
0x10C | Map Location
0x1C4E0 | Player Save Sort
0x5B1C | First Player of the save

Player Block
Offset | Name | DataType | Description
--- | --- | --- | --- 
0x00 | Player Index ID | UInt32 |
0x04 | Player ID | UInt32 |
0x0C | Gp | UInt16 |
0x0E | Tp | UInt16 |
0x10 | Freedom | UInt16 |
0x12 | Level | Byte |
0x14 | MixiMax Index ID | UInt32 |
0x20 | MixiMax Move 1 | Byte |
0x21 | MixiMax Move 2 | Byte |
0x22 | Spirit Information | Byte |
0x23 | Style | Byte |
0x2C | Kick Point | UInt16 |
0x2E | Dribble Point | UInt16 |
0x30 | Technique Point | UInt16 |
0x32 | Defense Point | UInt16 |
0x34 | Speed Point | UInt16 |
0x36 | Stamina Point | UInt16 |
0x38 | Catch Point | UInt16 |
0x3A | Luck Point | UInt16 |
0x3C | Fighting Spirit ID | UInt32 |
0x40 | Fighting Spirit Level | Byte |
0x44 | Boots Index ID | UInt32 |
0x48 | Bracelet Index ID | UInt32 |
0x50 | Gloves Index ID | UInt32 |
0x44 | Boots Index ID | UInt32 |
0x58 | Move 1 | UInt32 |
0x5C | Move 1 Level | Byte |
0x5D | Move 1 Usage | Byte |
0x5E | Move 1 Learned | Byte | 1 = True / 0 = False |
