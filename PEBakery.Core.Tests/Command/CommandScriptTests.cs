﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PEBakery.Helper;
using System;
using System.IO;
using System.Linq;
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace PEBakery.Core.Tests.Command
{
    [TestClass]
    public class CommandScriptTests
    {
        #region ExtractFile
        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("CommandScript")]
        public void ExtractFile()
        {
            EngineState s = EngineTests.CreateEngineState();
            string srcDir = StringEscaper.Preprocess(s, Path.Combine("%TestBench%", "EncodedFile"));
            string scPath = Path.Combine(srcDir, "ExtractFileTests.script");
            string destDir = Path.GetTempFileName();
            try
            {
                File.Delete(destDir);
                Directory.CreateDirectory(destDir);

                void SingleTemplate(string rawCode, string srcFileName, ErrorCheck check = ErrorCheck.Success)
                {
                    EngineTests.Eval(s, rawCode, CodeType.ExtractFile, check);

                    if (check == ErrorCheck.Success || check == ErrorCheck.Warning)
                    {
                        string srcFile = Path.Combine(srcDir, srcFileName);
                        string destFile = Path.Combine(destDir, srcFileName);

                        byte[] originDigest;
                        byte[] extractDigest;
                        using (FileStream fs = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            originDigest = HashHelper.GetHash(HashType.SHA256, fs);
                        }
                        using (FileStream fs = new FileStream(destFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            extractDigest = HashHelper.GetHash(HashType.SHA256, fs);
                        }
                        Assert.IsTrue(originDigest.SequenceEqual(extractDigest));
                    }
                }

                SingleTemplate($@"ExtractFile,{scPath},FolderExample,Type1.jpg,{destDir}", "Type1.jpg"); // Type 1
                SingleTemplate($@"ExtractFile,{scPath},FolderExample,Type2.7z,{destDir}", "Type2.7z"); // Type 2
                SingleTemplate($@"ExtractFile,{scPath},FolderExample,Type3.pdf,{destDir}", "Type3.pdf"); // Type 3
            }
            finally
            {
                if (Directory.Exists(destDir))
                    Directory.Delete(destDir, true);
            }
        }
        #endregion

        #region ExtractAllFiles
        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("CommandScript")]
        public void ExtractAllFiles()
        {
            EngineState s = EngineTests.CreateEngineState();
            string srcDir = StringEscaper.Preprocess(s, Path.Combine("%TestBench%", "EncodedFile"));
            string scPath = Path.Combine(srcDir, "ExtractFileTests.script");
            string destDir = Path.GetTempFileName();
            try
            {
                File.Delete(destDir);
                Directory.CreateDirectory(destDir);

                void SingleTemplate(string rawCode, string[] srcFileNames, ErrorCheck check = ErrorCheck.Success)
                {
                    EngineTests.Eval(s, rawCode, CodeType.ExtractAllFiles, check);
                    if (check == ErrorCheck.Success || check == ErrorCheck.Warning)
                    {
                        foreach (string srcFileName in srcFileNames)
                        {
                            string srcFile = Path.Combine(srcDir, srcFileName);
                            string destFile = Path.Combine(destDir, srcFileName);

                            byte[] originDigest;
                            byte[] extractDigest;
                            using (FileStream fs = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                originDigest = HashHelper.GetHash(HashType.SHA256, fs);
                            }
                            using (FileStream fs = new FileStream(destFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                extractDigest = HashHelper.GetHash(HashType.SHA256, fs);
                            }
                            Assert.IsTrue(originDigest.SequenceEqual(extractDigest));
                        }
                    }
                }

                SingleTemplate($@"ExtractAllFiles,{scPath},FolderExample,{destDir}", new string[] {
                    "Type1.jpg",
                    "Type2.7z",
                    "Type3.pdf"
                });
            }
            finally
            {
                if (Directory.Exists(destDir))
                    Directory.Delete(destDir, true);
            }
        }
        #endregion

        #region Encode
        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("CommandScript")]
        public void Encode()
        {
            const string folderName = "FolderExample";

            EngineState s = EngineTests.CreateEngineState();
            string srcDir = StringEscaper.Preprocess(s, Path.Combine("%TestBench%", "EncodedFile"));
            string blankScript = Path.Combine(srcDir, "Blank.script");

            string destDir = Path.GetTempFileName();
            try
            {
                File.Delete(destDir);
                Directory.CreateDirectory(destDir);

                string destScript = Path.Combine(destDir, Path.GetFileName(Path.GetRandomFileName()) + ".script");

                void SingleTemplate(string rawCode, string srcFileName, ErrorCheck check = ErrorCheck.Success)
                {
                    File.Copy(blankScript, destScript, true);

                    EngineTests.Eval(s, rawCode, CodeType.Encode, check);

                    if (check == ErrorCheck.Success || check == ErrorCheck.Warning)
                    {
                        string srcFile = Path.Combine(srcDir, srcFileName);
                        Script sc = s.Project.LoadScriptRuntime(destScript, new LoadScriptRuntimeOptions());

                        // Check whether file was successfully encoded
                        Assert.IsTrue(sc.Sections.ContainsKey(ScriptSection.Names.EncodedFolders));
                        string[] folders = sc.Sections[ScriptSection.Names.EncodedFolders].Lines
                            .Where(x => !x.Equals(string.Empty, StringComparison.Ordinal))
                            .ToArray();
                        Assert.IsTrue(folders.Length == 2);
                        Assert.IsTrue(folders[0].Equals(folderName, StringComparison.Ordinal));

                        Assert.IsTrue(sc.Sections.ContainsKey(folderName));
                        string[] fileInfos = sc.Sections[folderName].Lines
                            .Where(x => !x.Equals(string.Empty, StringComparison.Ordinal))
                            .ToArray();
                        Assert.IsTrue(fileInfos[0].StartsWith($"{srcFileName}=", StringComparison.Ordinal));

                        Assert.IsTrue(sc.Sections.ContainsKey(ScriptSection.Names.GetEncodedSectionName(folderName, srcFileName)));
                        string[] encodedFile = sc.Sections[ScriptSection.Names.GetEncodedSectionName(folderName, srcFileName)].Lines
                            .Where(x => x.Length != 0)
                            .ToArray();
                        Assert.IsTrue(1 < encodedFile.Length);
                        Assert.IsTrue(encodedFile[0].StartsWith("lines=", StringComparison.Ordinal));

                        // Check whether file can be successfully extracted
                        byte[] extractDigest;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            EncodedFile.ExtractFile(sc, folderName, srcFileName, ms, null);
                            ms.Position = 0;
                            extractDigest = HashHelper.GetHash(HashType.SHA256, ms);
                        }

                        byte[] originDigest;
                        using (FileStream fs = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            originDigest = HashHelper.GetHash(HashType.SHA256, fs);
                        }

                        Assert.IsTrue(originDigest.SequenceEqual(extractDigest));
                    }
                }

                SingleTemplate($@"Encode,{destScript},{folderName},{srcDir}\Type1.jpg,Deflate", "Type1.jpg"); // Type 1
                SingleTemplate($@"Encode,{destScript},{folderName},{srcDir}\Type2.7z,None", "Type2.7z"); // Type 2
                SingleTemplate($@"Encode,{destScript},{folderName},{srcDir}\Type3.pdf,LZMA2", "Type3.pdf"); // Type 3
                SingleTemplate($@"Encode,{destScript},{folderName},{srcDir}\PEBakeryAlphaMemory.jpg", "PEBakeryAlphaMemory.jpg");
            }
            finally
            {
                if (Directory.Exists(destDir))
                    Directory.Delete(destDir, true);
            }
        }
        #endregion
    }
}
