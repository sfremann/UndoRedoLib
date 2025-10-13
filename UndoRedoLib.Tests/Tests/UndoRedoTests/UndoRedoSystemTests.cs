// Copyright (c) 2025 Sarah Frémann.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using UndoRedoLib.UndoRedo;

namespace UndoRedoLib.Tests.Tests.UndoRedoTests
{
    [TestClass]
    public sealed class UndoRedoSystemTests
    {
        // --- Tests on UndoRedoSystem stacks

        [TestMethod]
        public void UndoRedoSystem_ChangeUndoStack_ShouldUpdateUndoDescription_AndUpdateUndoEmpty()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            string stubUndoEmptyDesc = undoRedoSystem.UndoDescription;
            UndoableAction action1 = new(() => { }, () => { }) { Description = "action1" };
            UndoableAction action2 = new(() => { }, () => { }) { Description = "action2" };

            // Act
            undoRedoSystem.ExecuteUndoableAction(action1);
            undoRedoSystem.ExecuteUndoableAction(action2);
            string undoDescription2 = undoRedoSystem.UndoDescription;
            bool undoEmpty2 = undoRedoSystem.UndoEmpty;
            undoRedoSystem.UndoCommand.Execute(null);
            string undoDescription1 = undoRedoSystem.UndoDescription;
            bool undoEmpty1 = undoRedoSystem.UndoEmpty;
            undoRedoSystem.UndoCommand.Execute(null);

            // Assert
            Assert.IsTrue(undoRedoSystem.UndoEmpty);
            Assert.IsFalse(undoEmpty1);
            Assert.IsFalse(undoEmpty2);
            Assert.AreEqual(stubUndoEmptyDesc, undoRedoSystem.UndoDescription);
            Assert.AreEqual(action1.Description, undoDescription1);
            Assert.AreEqual(action2.Description, undoDescription2);
        }

        [TestMethod]
        public void UndoRedoSystem_ChangeRedoStack_ShouldUpdateRedoDescription_AndUpdateRedoEmpty()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            string stubRedoEmptyDesc = undoRedoSystem.RedoDescription;
            UndoableAction action1 = new(() => { }, () => { }) { Description = "action1" };
            UndoableAction action2 = new(() => { }, () => { }) { Description = "action2" };

            // Act
            undoRedoSystem.ExecuteUndoableAction(action1);
            undoRedoSystem.ExecuteUndoableAction(action2);
            undoRedoSystem.UndoCommand.Execute(null);
            string redoDescription2 = undoRedoSystem.RedoDescription;
            bool redoEmpty2 = undoRedoSystem.RedoEmpty;
            undoRedoSystem.UndoCommand.Execute(null);
            string redoDescription1 = undoRedoSystem.RedoDescription;
            bool redoEmpty1 = undoRedoSystem.RedoEmpty;
            undoRedoSystem.ExecuteUndoableAction(action1);

            // Assert
            Assert.IsTrue(undoRedoSystem.RedoEmpty);
            Assert.IsFalse(redoEmpty1);
            Assert.IsFalse(redoEmpty2);
            Assert.AreEqual(stubRedoEmptyDesc, undoRedoSystem.RedoDescription);
            Assert.AreEqual(action1.Description, redoDescription1);
            Assert.AreEqual(action2.Description, redoDescription2);
        }

        // --- Tests on UndoRedoSystem.UndoRedoSystem(int MaxActionsInHistory)

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 1)]
        [DataRow(3, 2)]
        public void UndoRedoSystem_AddActionToUndoStack_ShouldEnsureMaxCapacity(int nbActions, int maxActions)
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new(maxActions);
            UndoableAction action = new(() => { }, () => { });

            // Act
            for (int i = 0; i < nbActions; i++)
            {
                undoRedoSystem.ExecuteUndoableAction(action);
            }
            for (int i = 0; i < maxActions; i++)
            {
                undoRedoSystem.UndoCommand.Execute(null);
            }

            // Assert
            Assert.IsTrue(undoRedoSystem.UndoEmpty);
        }

        // --- Tests on UndoRedoSystem.UndoDescription

        [TestMethod]
        public void UndoRedoSystem_SettingUndoDescriptionToDifferentValue_ShouldRaisePropertyChanged()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.UndoDescription))
                {
                    propertyChanged = true;
                }
            };

            // Act
            undoRedoSystem.UndoDescription = "description";

            // Assert
            Assert.IsTrue(propertyChanged);
        }

        [TestMethod]
        public void UndoRedoSystem_SettingUndoDescriptionToSameValue_ShouldNotRaisePropertyChanged()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.UndoDescription))
                {
                    propertyChanged = true;
                }
            };

            // Act
            undoRedoSystem.UndoDescription = undoRedoSystem.UndoDescription;

            // Assert
            Assert.IsFalse(propertyChanged);
        }

        // --- Tests on UndoRedoSystem.RedoDescription

        [TestMethod]
        public void UndoRedoSystem_SettingRedoDescriptionToDifferentValue_ShouldRaisePropertyChanged()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.RedoDescription))
                {
                    propertyChanged = true;
                }
            };

            // Act
            undoRedoSystem.RedoDescription = "description";

            // Assert
            Assert.IsTrue(propertyChanged);
        }

        [TestMethod]
        public void UndoRedoSystem_SettingRedoDescriptionToSameValue_ShouldNotRaisePropertyChanged()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.RedoDescription))
                {
                    propertyChanged = true;
                }
            };

            // Act
            undoRedoSystem.RedoDescription = undoRedoSystem.RedoDescription;

            // Assert
            Assert.IsFalse(propertyChanged);
        }

        // --- Tests on UndoRedoSystem.UndoEmpty

        [TestMethod]
        public void UndoRedoSystem_SettingUndoEmptyToDifferentValue_ShouldRaisePropertyChanged_AndRaiseCanExecuteChangedForUndoCommand()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            bool canExecuteChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.UndoEmpty))
                {
                    propertyChanged = true;
                }
            };
            undoRedoSystem.UndoCommand.CanExecuteChanged += (sender, e) =>
            {
                canExecuteChanged = true;
            };

            // Act
            undoRedoSystem.UndoEmpty = !undoRedoSystem.UndoEmpty;

            // Assert
            Assert.IsTrue(propertyChanged);
            Assert.IsTrue(canExecuteChanged);
        }

        [TestMethod]
        public void UndoRedoSystem_SettingUndoEmptyToSameValue_ShouldNotRaisePropertyChanged_AndNotRaiseCanExecuteChangedForUndoCommand()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            bool canExecuteChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.UndoEmpty))
                {
                    propertyChanged = true;
                }
            };
            undoRedoSystem.UndoCommand.CanExecuteChanged += (sender, e) =>
            {
                canExecuteChanged = true;
            };

            // Act
            undoRedoSystem.UndoEmpty = undoRedoSystem.UndoEmpty;

            // Assert
            Assert.IsFalse(propertyChanged);
            Assert.IsFalse(canExecuteChanged);
        }

        // --- Tests on UndoRedoSystem.RedoEmpty

        [TestMethod]
        public void UndoRedoSystem_SettingRedoEmptyToDifferentValue_ShouldRaisePropertyChanged_AndRaiseCanExecuteChangedForRedoCommand()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            bool canExecuteChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.RedoEmpty))
                {
                    propertyChanged = true;
                }
            };
            undoRedoSystem.RedoCommand.CanExecuteChanged += (sender, e) =>
            {
                canExecuteChanged = true;
            };

            // Act
            undoRedoSystem.RedoEmpty = !undoRedoSystem.RedoEmpty;

            // Assert
            Assert.IsTrue(propertyChanged);
            Assert.IsTrue(canExecuteChanged);
        }

        [TestMethod]
        public void UndoRedoSystem_SettingRedoEmptyToSameValue_ShouldNotRaisePropertyChanged_AndNotRaiseCanExecuteChangedForRedoCommand()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            bool canExecuteChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.RedoEmpty))
                {
                    propertyChanged = true;
                }
            };
            undoRedoSystem.RedoCommand.CanExecuteChanged += (sender, e) =>
            {
                canExecuteChanged = true;
            };

            // Act
            undoRedoSystem.RedoEmpty = undoRedoSystem.RedoEmpty;

            // Assert
            Assert.IsFalse(propertyChanged);
            Assert.IsFalse(canExecuteChanged);
        }

        // --- Tests on UndoRedoSystem.IsDirty

        [TestMethod]
        public void UndoRedoSystem_SettingIsDirtyToDifferentValue_ShouldRaisePropertyChanged_AndRaiseCanExecuteChangedForSaveCommand()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            bool canExecuteChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.IsDirty))
                {
                    propertyChanged = true;
                }
            };
            undoRedoSystem.SaveCommand.CanExecuteChanged += (sender, e) =>
            {
                canExecuteChanged = true;
            };

            // Act
            undoRedoSystem.IsDirty = !undoRedoSystem.IsDirty;

            // Assert
            Assert.IsTrue(propertyChanged);
            Assert.IsTrue(canExecuteChanged);
        }

        [TestMethod]
        public void UndoRedoSystem_SettingIsDirtyToSameValue_ShouldNotRaisePropertyChanged_AndNotRaiseCanExecuteChangedForSaveCommand()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool propertyChanged = false;
            bool canExecuteChanged = false;
            undoRedoSystem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UndoRedoSystem.IsDirty))
                {
                    propertyChanged = true;
                }
            };
            undoRedoSystem.SaveCommand.CanExecuteChanged += (sender, e) =>
            {
                canExecuteChanged = true;
            };

            // Act
            undoRedoSystem.IsDirty = undoRedoSystem.IsDirty;

            // Assert
            Assert.IsFalse(propertyChanged);
            Assert.IsFalse(canExecuteChanged);
        }

        // --- Tests on UndoRedoSystem.SaveCommand

        [TestMethod]
        public void UndoRedoSystem_ExecutingSaveCommand_ShouldSetIsDirtyToFalse()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new() { IsDirty = true };

            // Act
            undoRedoSystem.SaveCommand.Execute(null);

            // Assert
            Assert.IsFalse(undoRedoSystem.IsDirty);
        }

        [TestMethod]
        public void UndoRedoSystem_CanSave_IsDirty_ShouldBeTrue()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            bool stubCanSaveIsDirty = true;
            bool stubCanSaveNotIsDirty = false;
            bool canSaveNotIsDirty = undoRedoSystem.SaveCommand.CanExecute(null);

            // Act
            undoRedoSystem.IsDirty = true;

            // Assert
            Assert.AreEqual(stubCanSaveIsDirty, undoRedoSystem.SaveCommand.CanExecute(null));
            Assert.AreEqual(stubCanSaveNotIsDirty, canSaveNotIsDirty);
        }

        // --- Tests on UndoRedoSystem.UndoCommand

        [TestMethod]
        public void UndoRedoSystem_ExecutingUndoCommand_ShouldExecuteUndoForLastCommand_AndRemoveLastCommandFromUndoStack_AndAddLastCommandToRedoStack_AndSetIsDirtyToTrue()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new() { IsDirty = true };
            int stubUndoValue = 1;
            int stubRedoValue = -stubUndoValue;
            int undoValue = 0;
            int doValue = 0;
            UndoableAction action = new(() => { doValue = -undoValue; }, () => { undoValue = stubUndoValue; });
            undoRedoSystem.ExecuteUndoableAction(action);
            undoRedoSystem.IsDirty = false;

            // Act
            undoRedoSystem.UndoCommand.Execute(null);
            bool isDirty = undoRedoSystem.IsDirty;
            bool undoEmpty = undoRedoSystem.UndoEmpty;
            undoRedoSystem.RedoCommand.Execute(null);

            // Assert
            Assert.IsTrue(undoEmpty);
            Assert.AreEqual(stubRedoValue, doValue);
            Assert.AreEqual(stubUndoValue, undoValue);
            Assert.IsTrue(isDirty);
        }

        [TestMethod]
        public void UndoRedoSystem_CanUndo_UndoEmpty_ShouldBeFalse()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new() { UndoEmpty = false };
            bool canExecuteNotUndoEmpty = undoRedoSystem.UndoCommand.CanExecute(null);
            bool stubCanExecuteNotUndoEmpty = true;
            bool stubCanExecuteUndoEmpty = false;

            // Act
            undoRedoSystem.UndoEmpty = true;

            // Assert
            Assert.AreEqual(stubCanExecuteUndoEmpty, undoRedoSystem.UndoCommand.CanExecute(null));
            Assert.AreEqual(stubCanExecuteNotUndoEmpty, canExecuteNotUndoEmpty);
        }

        // --- Tests on UndoRedoSystem.RedoCommand

        [TestMethod]
        public void UndoRedoSystem_ExecutingRedoCommand_ShouldExecuteDoForLastCommand_AndRemoveLastCommandFromRedoStack_AndAddLastCommandToUndoStack_AndSetIsDirtyToTrue()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new() { IsDirty = true };
            int stubUndoValue = 1;
            int stubRedoValue = -stubUndoValue;
            int undoValue = 0;
            int doValue = 0;
            UndoableAction action = new(
                () => { doValue = -undoValue; undoValue = 0; },
                () => { undoValue = stubUndoValue; });
            undoRedoSystem.ExecuteUndoableAction(action);
            undoRedoSystem.UndoCommand.Execute(null);
            undoRedoSystem.IsDirty = false;

            // Act
            undoRedoSystem.RedoCommand.Execute(null);
            bool isDirty = undoRedoSystem.IsDirty;
            undoRedoSystem.UndoCommand.Execute(null);

            // Assert
            Assert.AreEqual(stubRedoValue, doValue);
            Assert.AreEqual(stubUndoValue, undoValue);
            Assert.IsTrue(isDirty);
        }

        [TestMethod]
        public void UndoRedoSystem_CanRedo_RedoEmpty_ShouldBeFalse()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new() { RedoEmpty = false };
            bool canExecuteNotRedoEmpty = undoRedoSystem.RedoCommand.CanExecute(null);
            bool stubCanExecuteNotRedoEmpty = true;
            bool stubCanExecuteRedoEmpty = false;

            // Act
            undoRedoSystem.RedoEmpty = true;

            // Assert
            Assert.AreEqual(stubCanExecuteRedoEmpty, undoRedoSystem.RedoCommand.CanExecute(null));
            Assert.AreEqual(stubCanExecuteNotRedoEmpty, canExecuteNotRedoEmpty);
        }

        // --- Tests on UndoRedoSystem.ClearHistoryCommand

        [TestMethod]
        public void UndoRedoSystem_ExecutingClearHistoryCommand_ShouldEmptyUndoAndRedoStacks_AndSetIsDirtyToFalse()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            UndoableAction action = new(() => { }, () => { });
            undoRedoSystem.ExecuteUndoableAction(action);
            undoRedoSystem.ExecuteUndoableAction(action);
            undoRedoSystem.UndoCommand.Execute(null);

            // Act
            undoRedoSystem.ClearHistoryCommand.Execute(null);

            // Assert
            Assert.IsTrue(undoRedoSystem.UndoEmpty);
            Assert.IsTrue(undoRedoSystem.RedoEmpty);
            Assert.IsFalse(undoRedoSystem.IsDirty);
        }

        // --- Tests on UndoRedoSystem.ExecuteUndoableAction(UndoableAction action, bool skipExecution = false)

        [TestMethod]
        public void UndoRedoSystem_ExecuteUndoableAction_SkipExecution_ShouldAddActionToUndoStack_AndNotExectuteDo_AndSetIsDirtyToTrue_AndEmptyRedoStack()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            int stubUndoValue = 1;
            int undoValue = 0;
            int doValue = 0;
            int stubDoValue = doValue;
            UndoableAction action = new(
                () => { doValue++; },
                () => { undoValue = stubUndoValue; });

            // Act
            undoRedoSystem.ExecuteUndoableAction(action, true /* skipExecution */);
            bool isDirty = undoRedoSystem.IsDirty;
            undoRedoSystem.UndoCommand.Execute(null);
            undoRedoSystem.ExecuteUndoableAction(new(() => { }, () => { }), true /* skipExecution */);

            // Assert
            Assert.AreEqual(stubDoValue, doValue);
            Assert.AreEqual(stubUndoValue, undoValue);
            Assert.IsTrue(isDirty);
            Assert.IsTrue(undoRedoSystem.RedoEmpty);
        }

        [TestMethod]
        public void UndoRedoSystem_ExecuteUndoableAction_ShouldAddActionToUndoStack_AndExectuteDo_AndSetIsDirtyToTrue_AndEmptyRedoStack()
        {
            // Arrange
            UndoRedoSystem undoRedoSystem = new();
            int stubDoValue = 1;
            int stubUndoValue = -stubDoValue;
            int undoValue = 0;
            int doValue = 0;
            UndoableAction action = new(
                () => { doValue = stubDoValue; },
                () => { undoValue = stubUndoValue; });

            // Act
            undoRedoSystem.ExecuteUndoableAction(action);
            bool isDirty = undoRedoSystem.IsDirty;
            undoRedoSystem.UndoCommand.Execute(null);
            undoRedoSystem.ExecuteUndoableAction(new(() => { }, () => { }));

            // Assert
            Assert.AreEqual(stubDoValue, doValue);
            Assert.AreEqual(stubUndoValue, undoValue);
            Assert.IsTrue(isDirty);
            Assert.IsTrue(undoRedoSystem.RedoEmpty);
        }
    }
}
