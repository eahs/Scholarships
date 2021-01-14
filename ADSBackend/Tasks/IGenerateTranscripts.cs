namespace Scholarships.Tasks
{
    // Reads an existing pdf of ALL EAHS seniors and their transcripts and turns it into individual transcripts (one per file)
    public interface IGenerateTranscripts
    {
        void Execute();
    }
}
