# UndoRedoLib
WPF class library with basic undo-redo functionality

## Undoable Action

Struct with two delegates: one for the *do* action, the other for the *undo* action.

Usage:
```
UndoableAction action = new(() => { /* Do */ }, () => { /* Undo */ })
{
    Description = "description" // Optional
};
action.Do();
action.Undo();
```

## Undo-Redo System

Observable object that can be used as a view model to provide a history manager.

Usage:
```
UndoRedoSystem history = new(100 /* Optional max actions in history, default is 100 */);

UndoableAction action = new(() => { /* Do */ }, () => { /* Undo */ })
{
    Description = "description" // Optional
};
history.ExecuteUndoableAction(action, false /* Optional skip execution, default is false */);

history.UndoCommand.Execute(null);
history.RedoCommand.Execute(null);
```

### Status information

- Undo command tooltip: `UndoRedoSystem.UndoDescription`
- Redo command tooltip: `UndoRedoSystem.RedoDescription`
- Undo stack status: `UndoRedoSystem.UndoEmpty`
- Redo stack status: `UndoRedoSystem.RedoEmpty`
- Dirty status: `UndoRedoSystem.IsDirty`

### Save commands

- Save: `UndoRedoSystem.SaveCommand.Execute(null)` or `UndoRedoSystem.SaveCommand.ExecuteAsync(null)`
- Clear history: `UndoRedoSystem.ClearHistoryCommand.Execute(null)`

When inheriting from UndoRedoSystem, the child class can override `UndoRedoSystem.SaveState()` to change the save command behavior:
```
class ViewModel : UndoRedoSystem
{
    protected override async Task SaveState()
    {
        await base.SaveState();

        /* Additional behavior */
    }
}
```