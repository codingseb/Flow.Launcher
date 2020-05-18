using Flow.Launcher.Plugin.Explorer;
using Flow.Launcher.Plugin.Explorer.Search.WindowsIndex;
using NUnit.Framework;
using System;

namespace Flow.Launcher.Test.Plugins
{
    [TestFixture]
    public class ExplorerTest
    {
        [TestCase("C:\\Dropbox", "directory='file:C:\\Dropbox'")]
        public void GivenWindowsIndexSearch_WhenProvidedFolderPath_ThenQueryWhereRestrictionsShouldUseDirectoryString(string path, string expectedString)
        {
            // Given
            var queryConstructor = new QueryConstructor(new Settings());

            // When
            var folderPath = path;
            var result = queryConstructor.QueryWhereRestrictionsForTopLevelDirectorySearch(folderPath);

            // Then
            Assert.IsTrue(result == expectedString,
                $"Expected QueryWhereRestrictions string: {expectedString}{Environment.NewLine} " +
                $"Actual: {result}{Environment.NewLine}");
        }

        [TestCase("C:\\Dropbox", "SELECT TOP 100 System.FileName, System.ItemPathDisplay FROM SystemIndex WHERE directory='file:C:\\Dropbox'")]
        public void GivenWindowsIndexSearch_WhenSearchTypeIsSearchTopFolderLevel_ThenQueryShouldUseExpectedString(string folderPath, string expectedString)
        {
            // Given
            var queryConstructor = new QueryConstructor(new Settings());
            
            //When            
            var queryString = queryConstructor.QueryForTopLevelDirectorySearch(folderPath);
            
            Assert.IsTrue(queryString == expectedString,
                $"Expected QueryWhereRestrictions string: {expectedString}{Environment.NewLine} " +
                $"Actual string was: {queryString}{Environment.NewLine}");
        }

        [TestCase("scope='file:'")]
        public void GivenWindowsIndexSearch_WhenSearchAllFoldersAndFiles_ThenQueryWhereRestrictionsShouldUseScopeString(string expectedString) 
        {
            // Given
            var queryConstructor = new QueryConstructor(new Settings());

            //When
            var resultString = queryConstructor.QueryWhereRestrictionsForAllFilesAndFoldersSearch();

            Assert.IsTrue(resultString == expectedString,
                $"Expected QueryWhereRestrictions string: {expectedString}{Environment.NewLine} " +
                $"Actual string was: {resultString}{Environment.NewLine}");
        }

        [TestCase("flow.launcher.sln", "SELECT TOP 100 \"System.FileName\", \"System.ItemPathDisplay\" " +
            "FROM \"SystemIndex\" WHERE (System.FileName LIKE 'flow.launcher.sln%' " +
                                        "OR CONTAINS(System.FileName,'\"flow.launcher.sln*\"',1033)) AND scope='file:'")]
        public void GivenWindowsIndexSearch_WhenSearchAllFoldersAndFiles_ThenQueryShouldUseExpectedString(
            string userSearchString, string expectedString) 
        {
            // Given
            var queryConstructor = new QueryConstructor(new Settings());

            //When
            var resultString = queryConstructor.QueryForAllFilesAndFolders(userSearchString);

            Assert.IsTrue(resultString == expectedString,
                $"Expected query string: {expectedString}{Environment.NewLine} " +
                $"Actual string was: {resultString}{Environment.NewLine}");
        }

        public void GivenWindowsIndexSearch_WhenReturnedNilAndIsNotIndexed_ThenSearchMethodShouldContinueDirectoryInfoClassSearch() { }

        public void GivenWindowsIndexSearch_WhenSearchPatternHotKeyIsSearchAll_ThenQueryWhereRestrictionsShouldUseScopeString() { }
    }
}