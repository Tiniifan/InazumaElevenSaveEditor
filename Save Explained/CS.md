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
0x12 | Level | Byte |
0x14 | Player MixiMax Index ID | UInt32 |
0x58 | Move 1 | UInt32 |
0x5C | Move 1 Level | Byte |
0x5D | Move 1 Usage | Byte |
0x5E | Move 1 Learned | Byte | 1 = True / 0 = False |
