namespace MusicalScalePlayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var musicSequence = new List<MusicScale>();
            var musicSound = new Dictionary<MusicalNote,int>()
            {
                {MusicalNote.A, 440 },
                {MusicalNote.A_, 466 },
                {MusicalNote.B, 493 },
                {MusicalNote.C, 261 },
                {MusicalNote.C_, 277 },
                {MusicalNote.D, 293 },
                {MusicalNote.D_, 311 },
                {MusicalNote.E, 329 },
                {MusicalNote.F, 349 },
                {MusicalNote.F_, 369 },
                {MusicalNote.G, 392 },
                {MusicalNote.G_, 415 }
            };

            Console.WriteLine("Available Notes: A A# B C C# D D# E F F# G G#" +
                                "Enter sequence");
            string input = Console.ReadLine()!.ToUpper() ?? string.Empty;
            string[] sequence = input.Split(" ");
            foreach (string sequenceItem in sequence)
            {
                switch (sequenceItem)
                {
                    case "A":
                        musicSequence.Add(new MusicScale(MusicalNote.A));
                        break;
                    case "A#":
                        musicSequence.Add(new MusicScale(MusicalNote.A_));
                        break;
                    case "B":
                        musicSequence.Add(new MusicScale(MusicalNote.B));
                        break;
                    case "C":
                        musicSequence.Add(new MusicScale(MusicalNote.C));
                        break;
                    case "C#":
                        musicSequence.Add(new MusicScale(MusicalNote.C_));
                        break;
                    case "D":
                        musicSequence.Add(new MusicScale(MusicalNote.D));
                        break;
                    case "D#":
                        musicSequence.Add(new MusicScale(MusicalNote.D_));
                        break;
                    case "E":
                        musicSequence.Add(new MusicScale(MusicalNote.E));
                        break;
                    case "F":
                        musicSequence.Add(new MusicScale(MusicalNote.F));
                        break;
                    case "F#":
                        musicSequence.Add(new MusicScale(MusicalNote.F_));
                        break;
                    case "G":
                        musicSequence.Add(new MusicScale(MusicalNote.G));
                        break;
                    case "G#":
                        musicSequence.Add(new MusicScale(MusicalNote.G_));
                        break;
                    default:
                        Console.WriteLine($"Invalid note. Skipping {sequenceItem}");
                        break;
                }
            }

            foreach(var item in musicSequence)
            {
                Console.Beep(musicSound[item.MusicalNote], 250);
            }
        }
    }
}
