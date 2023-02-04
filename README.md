# DiffFinder

DiffFinder (from **[rajeevboobna / shelvesetcomparer](https://github.com/rajeevboobna/shelvesetcomparer)**) extends the existing **[hamidshahid / shelvesetcomparer](https://github.com/hamidshahid/shelvesetcomparer)** Visual Studio extension.
ShelvesetComparer is a Visual Studio extension that allow users to compare contents of two shelvesets (from on or different users).

**DiffFinder** extends it to also

* allow comparison of a shelveset with current workspace local pending changes
* compare shelvesets targeting different branches, e.g. `$/BranchA/path/to/file1` with `$/BranchB/path/to/file1`

In the comparison list following color coding is used:

* **black**: no difference / both files are identical
* **red**: differences / both files differ
* **blue**: file exists only in one of the compared containers (Shelveset or Pending Changes)

## VisualStudio Marketplace

* Previous versions:
   1. [DiffFinder (VS2017)](https://marketplace.visualstudio.com/items?itemName=RajeevBoobna.DiffFinder)
   1. [ShelvesetComparer (VS2017)](https://marketplace.visualstudio.com/items?itemName=HamidShahid.ShelvesetComparer-19329)

## Examples

1. Open Shelveset selection from TeamExplorer:
![TeamExplorer Diff Finder Button](/DiffFinder/Resources/PreviewImage.png)

2. Select two Shelvesets for comparison or one Shelvesets to compare with current Pending Changes:
![TeamExplorer Diff Finder Button](/DiffFinder/Resources/Example_SelectCompareShelvesets.png)

3. File comparison view comparing two Shelvesets:

   * Shelvesets with same file paths (same branch):
![TeamExplorer Diff Finder Button](/DiffFinder/Resources/Example_CompareSameBranch.png)

   * Shelvesets with different paths (algorithm tries to find the best match (most common path parts)):
      * differerent branches but same relative path:
      ![TeamExplorer Diff Finder Button](/DiffFinder/Resources/Example_CompareDifferentBranches.png)

      * different branches and different paths:
      ![File comparison: different branches and folders -> tries to find best match](/DiffFinder/Resources/Example_CompareDifferentBranchesAndFolders.png)
