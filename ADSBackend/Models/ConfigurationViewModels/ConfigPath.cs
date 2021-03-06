﻿using System.IO;

namespace Scholarships.Models.ConfigurationViewModels
{
    public class ConfigPath
    {
        public string JobOutputPath { get; set; } = Path.Combine("App_Data", "Jobs");
        public string DataProtectionKeys { get; set; } = Path.Combine("App_Data", "DataProtectionKeys");
        public string LogPath { get; set; } = Path.Combine("App_Data", "Logs");
        public string TranscriptsPath { get; set; } = Path.Combine("App_Data", "Transcripts");
        public string StudentDataPath { get; set; } = Path.Combine("App_Data", "StudentData");
        public string AttachmentPath { get; set; } = Path.Combine("App_Data", "Attachments");

    }
}
