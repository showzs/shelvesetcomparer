﻿// <copyright file="ShelvesetComparerNavigationLinkItem.cs" company="http://shelvesetcomparer.codeplex.com">Copyright http://shelvesetcomparer.codeplex.com. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace WiredTechSolutions.ShelvesetComparer
{
    using System;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.Reflection;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// The class creates the navigation link for Shelveset Comparer extension.
    /// </summary>
    [TeamExplorerNavigationItem(ShelvesetComparerNavigationLinkItem.LinkId, 1500)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShelvesetComparerNavigationLinkItem : TeamExplorerBaseNavigationItem
    {
        /// <summary>
        /// The unique Id given to the navigation link
        /// </summary>
        public const string LinkId = "A1EC4AFD-FEBF-499B-82F7-E51F987D30D2";

        /// <summary>
        /// Initializes a new instance of the ShelvesetComparerNavigationLinkItem class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        [ImportingConstructor]
        public ShelvesetComparerNavigationLinkItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Text = Resources.TeamExplorerLinkCaption;
            this.Image = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Resources.TeamExplorerIconName));
        }

        /// <summary>
        /// Overridden method called when the navigation link is clicked.
        /// </summary>
        public override void Execute()
        {
            ShelvesetComparer.EnsurePackageIsLoaded(base.ServiceProvider);
            TeamExplorer.NavigateToShelvesetComparer();
        }

        /// <summary>
        /// Overridden method called when the navigation link is refreshed.
        /// </summary>
        public override void Invalidate()
        {
            base.Invalidate();
        }
    }
}
