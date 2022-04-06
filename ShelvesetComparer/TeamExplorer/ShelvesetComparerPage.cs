﻿// <copyright file="ShelvesetComparerPage.cs" company="http://shelvesetcomparer.codeplex.com">Copyright http://shelvesetcomparer.codeplex.com. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace WiredTechSolutions.ShelvesetComparer
{
    using Microsoft.TeamFoundation.Controls;

    /// <summary>
    /// The class creates the team explorer page for Shelveset Comparer extension.
    /// </summary>
    [TeamExplorerPage(ShelvesetComparerPage.PageId, Undockable = true)]
    public class ShelvesetComparerPage : TeamExplorerBasePage
    {
        /// <summary>
        /// The unique id of the Team explorer page
        /// </summary>
        public const string PageId = "{9A984CF5-9D99-4C24-BDCB-E53A0D174343}";

        /// <summary>
        /// Initializes a new instance of the ShelvesetComparerPage class
        /// </summary>
        public ShelvesetComparerPage()
        {
            this.Title = Resources.ToolWindowTitle;
        }
    }
}
