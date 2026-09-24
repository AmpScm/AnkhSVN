using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Ankh;
using Ankh.Selection;
using Ankh.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Selection
{
    [TestFixture]
    public class SelectionItemMapContractTests
    {
        sealed class Wrapper
        {
            public Wrapper(string value) { Value = value; }
            public string Value { get; }
        }

        sealed class Owner : ISelectionMapOwner<string>
        {
            readonly ArrayList _selection = new ArrayList();
            readonly ArrayList _all = new ArrayList();
            readonly Dictionary<string, Wrapper> _wrappers =
                new Dictionary<string, Wrapper>(StringComparer.Ordinal);

            public Owner(params string[] items)
            {
                foreach (string item in items)
                    _all.Add(item);
            }

            public IList Selection => _selection;
            public IList AllItems => _all;
            public Control Control { get; } = new Panel();
            public event EventHandler HandleDestroyed;

            public bool SelectionContains(string item) => _selection.Contains(item);
            public IntPtr GetImageList() => new IntPtr(1234);
            public int GetImageListIndex(string item) => item == null ? -1 : item.Length;
            public string GetText(string item) => item == null ? null : "text:" + item;
            public string GetCanonicalName(string item) => item == null ? null : "file:///" + item;

            public object GetSelectionObject(string item)
            {
                if (!_wrappers.TryGetValue(item, out Wrapper wrapper))
                    _wrappers[item] = wrapper = new Wrapper(item);
                return wrapper;
            }

            public string GetItemFromSelectionObject(object item)
            {
                return item is Wrapper wrapper ? wrapper.Value : item as string;
            }

            public void SetSelection(string[] items)
            {
                _selection.Clear();
                if (items != null)
                    _selection.AddRange(items);
            }

            public void Select(params string[] items) => SetSelection(items);
            public void RaiseHandleDestroyed() => HandleDestroyed?.Invoke(this, EventArgs.Empty);
        }

        static SelectionItemMap Create(out Owner owner)
        {
            owner = new Owner("alpha", "beta", "gamma");
            owner.Select("alpha", "beta");
            return SelectionItemMap.Create(owner);
        }

        [Test]
        public void Create_RejectsNullOwner()
        {
            Assert.Throws<ArgumentNullException>(() =>
                SelectionItemMap.Create<string>(null));
        }

        [Test]
        public void MultiItemSelection_ReportsCountHierarchyAndBounds()
        {
            SelectionItemMap map = Create(out Owner owner);
            IVsMultiItemSelect multi = map;

            Assert.That(multi.GetSelectionInfo(out uint count, out int oneHierarchy),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(count, Is.EqualTo(2));
            Assert.That(oneHierarchy, Is.EqualTo(1));

            var selected = new VSITEMSELECTION[2];
            Assert.That(multi.GetSelectedItems(0, 2, selected), Is.EqualTo(VSErr.S_OK));
            Assert.That(selected[0].pHier, Is.Not.Null);
            Assert.That(selected[1].pHier, Is.SameAs(selected[0].pHier));
            Assert.That(selected[0].itemid, Is.Not.EqualTo(selected[1].itemid));

            var omitted = new VSITEMSELECTION[2];
            Assert.That(
                multi.GetSelectedItems((uint)__VSGSIFLAGS.GSI_fOmitHierPtrs, 2, omitted),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(omitted[0].pHier, Is.Null);
            Assert.That(omitted[1].pHier, Is.Null);

            Assert.That(multi.GetSelectedItems(0, 3, new VSITEMSELECTION[3]),
                Is.EqualTo(VSErr.E_FAIL));
            Assert.That(multi.GetSelectedItems(0, 2, new VSITEMSELECTION[1]),
                Is.EqualTo(VSErr.E_FAIL));
        }

        [Test]
        public void SelectionContainer_CountGetAndSelectObjectsHonorFlagsAndCache()
        {
            SelectionItemMap map = Create(out Owner owner);
            ISelectionContainer container = map;

            Assert.That(container.CountObjects((uint)Constants.GETOBJS_ALL, out uint all),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(all, Is.EqualTo(3));
            Assert.That(container.CountObjects((uint)Constants.GETOBJS_SELECTED, out uint selected),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(selected, Is.EqualTo(2));
            Assert.That(container.CountObjects(999, out uint invalid), Is.EqualTo(VSErr.E_FAIL));
            Assert.That(invalid, Is.Zero);

            Assert.That(container.GetObjects((uint)Constants.GETOBJS_SELECTED, 1, null),
                Is.EqualTo(VSErr.E_POINTER));
            Assert.That(container.GetObjects(999, 0, Array.Empty<object>()),
                Is.EqualTo(VSErr.E_FAIL));
            Assert.That(container.GetObjects((uint)Constants.GETOBJS_SELECTED, 3, new object[3]),
                Is.EqualTo(VSErr.E_FAIL));

            var first = new object[2];
            Assert.That(container.GetObjects((uint)Constants.GETOBJS_SELECTED, 2, first),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(((Wrapper)first[0]).Value, Is.EqualTo("alpha"));
            Assert.That(((Wrapper)first[1]).Value, Is.EqualTo("beta"));

            var second = new object[2];
            Assert.That(container.GetObjects((uint)Constants.GETOBJS_SELECTED, 2, second),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(second[0], Is.SameAs(first[0]));
            Assert.That(second[1], Is.SameAs(first[1]));

            var allObjects = new object[3];
            Assert.That(container.GetObjects((uint)Constants.GETOBJS_ALL, 3, allObjects),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(((Wrapper)allObjects[2]).Value, Is.EqualTo("gamma"));

            map.AllIsSelected = true;
            var selectedAsAll = new object[2];
            Assert.That(container.GetObjects((uint)Constants.GETOBJS_ALL, 2, selectedAsAll),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(((Wrapper)selectedAsAll[0]).Value, Is.EqualTo("alpha"));

            Assert.That(container.SelectObjects(1, null, 0), Is.EqualTo(VSErr.E_POINTER));

            var replacement = new object[] { owner.GetSelectionObject("gamma") };
            Assert.That(container.SelectObjects(1, replacement, 0), Is.EqualTo(VSErr.S_OK));
            Assert.That(owner.Selection, Is.EqualTo(new[] { "gamma" }));

            Assert.That(container.SelectObjects(0, null, 0), Is.EqualTo(VSErr.S_OK));
            Assert.That(owner.Selection, Is.Empty);
        }

        [Test]
        public void Hierarchy_MapsPropertiesCanonicalNamesAndEventNotifications()
        {
            SelectionItemMap map = Create(out Owner owner);
            IVsMultiItemSelect multi = map;
            var selected = new VSITEMSELECTION[1];

            Assert.That(multi.GetSelectedItems(0, 1, selected), Is.EqualTo(VSErr.S_OK));
            IVsHierarchy hierarchy = selected[0].pHier;
            uint id = selected[0].itemid;

            Assert.That(hierarchy.GetCanonicalName(id, out string canonical), Is.EqualTo(VSErr.S_OK));
            Assert.That(canonical, Is.EqualTo("file:///alpha"));

            Assert.That(hierarchy.GetCanonicalName(98765, out string unknown), Is.EqualTo(VSErr.S_OK));
            Assert.That(unknown, Is.EqualTo("{98765}"));

            Assert.That(hierarchy.ParseCanonicalName("{42}", out uint parsed), Is.EqualTo(VSErr.S_OK));
            Assert.That(parsed, Is.EqualTo(42));
            Assert.That(hierarchy.QueryClose(out int canClose), Is.EqualTo(VSErr.S_OK));
            Assert.That(canClose, Is.EqualTo(1));

            var props = new Dictionary<int, object>
            {
                [(int)__VSHPROPID.VSHPROPID_Parent] = VSItemId.Nil,
                [(int)__VSHPROPID.VSHPROPID_Root] = VSItemId.Root,
                [(int)__VSHPROPID.VSHPROPID_TypeGuid] = typeof(SelectionItemMap).GUID,
                [(int)__VSHPROPID.VSHPROPID_CmdUIGuid] = Guid.Empty,
                [(int)__VSHPROPID.VSHPROPID_Caption] = "text:alpha",
                [(int)__VSHPROPID.VSHPROPID_IconImgList] = 1234,
                [(int)__VSHPROPID.VSHPROPID_IconIndex] = 5,
                [(int)__VSHPROPID.VSHPROPID_Expandable] = false,
                [(int)__VSHPROPID.VSHPROPID_StateIconIndex] = (int)VsStateIcon.STATEICON_NOSTATEICON,
                [(int)__VSHPROPID.VSHPROPID_ParentHierarchy] = null
            };

            foreach (KeyValuePair<int, object> pair in props)
            {
                Assert.That(hierarchy.GetProperty(id, pair.Key, out object value), Is.EqualTo(VSErr.S_OK));
                Assert.That(value, Is.EqualTo(pair.Value));
            }

            Assert.That(hierarchy.GetProperty(VSItemId.Root, (int)__VSHPROPID.VSHPROPID_Caption, out object rootText),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(rootText, Is.EqualTo("."));
            Assert.That(hierarchy.GetProperty(99999, (int)__VSHPROPID.VSHPROPID_Caption, out object missing),
                Is.EqualTo(VSErr.E_FAIL));
            Assert.That(missing, Is.Null);
            Assert.That(hierarchy.GetProperty(id, 123456789, out object unsupported),
                Is.EqualTo(VSErr.E_FAIL));
            Assert.That(unsupported, Is.Null);

            Assert.That(hierarchy.GetGuidProperty(id, (int)__VSHPROPID.VSHPROPID_TypeGuid, out Guid typeGuid),
                Is.EqualTo(VSErr.S_OK));
            Assert.That(typeGuid, Is.EqualTo(typeof(SelectionItemMap).GUID));
            Assert.That(hierarchy.GetGuidProperty(id, (int)__VSHPROPID.VSHPROPID_Caption, out Guid noGuid),
                Is.EqualTo(VSErr.E_FAIL));
            Assert.That(noGuid, Is.EqualTo(Guid.Empty));

            Assert.That(hierarchy.AdviseHierarchyEvents(null, out uint nullCookie), Is.EqualTo(VSErr.E_POINTER));
            Assert.That(nullCookie, Is.Zero);

            var events = new Mock<IVsHierarchyEvents>();
            events.Setup(e => e.OnItemAdded(It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()))
                .Returns(VSErr.S_OK);
            Assert.That(hierarchy.AdviseHierarchyEvents(events.Object, out uint cookie), Is.EqualTo(VSErr.S_OK));
            Assert.That(cookie, Is.Not.Zero);

            owner.Select("alpha", "gamma");
            var refreshed = new VSITEMSELECTION[2];
            Assert.That(multi.GetSelectedItems(0, 2, refreshed), Is.EqualTo(VSErr.S_OK));
            events.Verify(e => e.OnItemAdded(VSItemId.Root, VSItemId.Root, It.IsAny<uint>()), Times.AtLeastOnce);

            Assert.That(hierarchy.UnadviseHierarchyEvents(cookie), Is.EqualTo(VSErr.S_OK));
            Assert.That(hierarchy.Close(), Is.EqualTo(VSErr.S_OK));
        }

        [Test]
        public void ContextAndPublishHierarchy_AreMutableWithoutAVisualStudioTracker()
        {
            SelectionItemMap map = Create(out Owner owner);

            Assert.That(map.Context, Is.Null);
            Assert.That(map.PublishHierarchy, Is.True);

            map.PublishHierarchy = false;
            Assert.That(map.PublishHierarchy, Is.False);

            map.NotifySelectionUpdated();
            map.EnsureSelection();

            owner.RaiseHandleDestroyed();
        }
    }
}
