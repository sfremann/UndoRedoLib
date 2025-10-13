// Copyright (c) 2025 Sarah Frémann.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace UndoRedoLib.UndoRedo
{
    /// <summary>
    /// Base view model with undo-redo functionality
    /// </summary>
    public partial class UndoRedoSystem : ObservableObject
    {
        // --- Properties

        private readonly ObservableCollection<UndoableAction> _undoStack = [];
        private readonly ObservableCollection<UndoableAction> _redoStack = [];
        private readonly int _maxActionsInHistory = 100;

        [ObservableProperty]
        private string _undoDescription = "";
        [ObservableProperty]
        private string _redoDescription = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UndoCommand))]
        private bool _undoEmpty = true;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RedoCommand))]
        private bool _redoEmpty = true;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private bool _isDirty = false;

        // --- Constructors

        public UndoRedoSystem()
        {
            _undoStack.CollectionChanged += UndoStack_CollectionChanged;
            _redoStack.CollectionChanged += RedoStack_CollectionChanged;
        }

        public UndoRedoSystem(int MaxActionsInHistory)
        {
            _maxActionsInHistory = MaxActionsInHistory >= 0 ? MaxActionsInHistory : 100;
            _undoStack.CollectionChanged += UndoStack_CollectionChanged;
            _redoStack.CollectionChanged += RedoStack_CollectionChanged;
        }

        // --- Commands

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            await SaveState();
        }

        private bool CanSave()
        {
            return IsDirty;
        }

        [RelayCommand(CanExecute = nameof(CanUndo))]
        private void Undo()
        {
            // Update history and undo last action
            var action = _undoStack.Last();
            action.Undo();
            _redoStack.Add(action);
            _undoStack.Remove(action);
            IsDirty = true;
        }

        private bool CanUndo()
        {
            return !UndoEmpty;
        }

        [RelayCommand(CanExecute = nameof(CanRedo))]
        private void Redo()
        {
            // Update history and redo last action
            var action = _redoStack.Last();
            ExecuteUndoableActionInternal(action);
            _redoStack.Remove(action);
        }

        private bool CanRedo()
        {
            return !RedoEmpty;
        }

        [RelayCommand]
        private void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            IsDirty = false;
        }

        // --- Events

        /// <summary>
        /// Update UndoEmpty and UndoDescription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoStack_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UndoEmpty = _undoStack.Count == 0;
            UndoDescription = UndoEmpty ? "" : _undoStack.Last().Description;

            // Ensure stack max capacity
            if (_undoStack.Count > _maxActionsInHistory)
            {
                _undoStack.RemoveAt(0);
            }
        }

        /// <summary>
        /// Update RedoEmpty and RedoDescription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoStack_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RedoEmpty = _redoStack.Count == 0;
            RedoDescription = RedoEmpty ? "" : _redoStack.Last().Description;
        }

        // --- Methods

        /// <summary>
        /// Save function to override to change the behavior of SaveCommand
        /// </summary>
        /// <returns></returns>
        protected virtual async Task SaveState()
        {
            IsDirty = false;
        }

        private void ExecuteUndoableActionInternal(UndoableAction action, bool skipExecution = false)
        {
            if (!skipExecution)
            {
                action.Do();
            }
            _undoStack.Add(action);
            IsDirty = true;
        }

        /// <summary>
        /// Update history and do action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="skipExecution"></param>
        public void ExecuteUndoableAction(UndoableAction action, bool skipExecution = false)
        {
            _redoStack.Clear();
            ExecuteUndoableActionInternal(action, skipExecution);
        }
    }
}