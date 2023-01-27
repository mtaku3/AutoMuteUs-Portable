using System.Reactive.Subjects;
using AutoMuteUsPortable.Shared.Entity.ProgressInfo;

namespace AutoMuteUsPortable.Shared.Utility;

public class TaskProgress
{
    public readonly TaskProgress? Parent;
    public readonly List<TaskProgress> Tasks = new();
    private object? _progress;
    private int _taskIdx = -1;

    public string Name = "";

    public TaskProgress(ISubject<ProgressInfo> progress)
    {
        _progress = progress;
    }

    public TaskProgress(TaskProgress parent)
    {
        Parent = parent;
    }

    public TaskProgress(TaskProgress parent, Dictionary<string, object?> tasks)
    {
        Parent = parent;
        InitializeTasks(tasks);
    }

    public TaskProgress(TaskProgress parent, List<string> tasks)
    {
        Parent = parent;
        InitializeTasks(tasks);
    }

    public TaskProgress(ISubject<ProgressInfo> progress, Dictionary<string, object?> tasks)
    {
        _progress = progress;
        InitializeTasks(tasks);
        ActivateTasks();
    }

    public TaskProgress(ISubject<ProgressInfo> progress, List<string> tasks)
    {
        _progress = progress;
        InitializeTasks(tasks);
        ActivateTasks();
    }

    private int taskIdx
    {
        get => _taskIdx;
        set
        {
            _taskIdx = value;
            if (IsCompleted) DisposeProgress();
        }
    }

    public bool IsRoot => Parent == null;
    public bool IsLeafNode => Tasks.Count == 0;
    public bool IsCompleted => Tasks.Count <= taskIdx;
    public bool IsActive => IsRoot || Parent!.Task == this;
    public TaskProgress? Task => taskIdx == -1 || Tasks.Count <= taskIdx ? null : Tasks[taskIdx];

    public TaskProgress? ActiveLeafTask
    {
        get
        {
            if (!IsActive) return null;
            if (IsCompleted) return null;
            if (!IsRoot && IsLeafNode) return this;

            return Task?.ActiveLeafTask;
        }
    }

    private void InitializeTasks(Dictionary<string, object?> tasks)
    {
        foreach (var (key, value) in tasks)
            if (value is Dictionary<string, object?> dict)
            {
                var task = new TaskProgress(this, dict)
                {
                    Name = key
                };
                var progress = new Subject<ProgressInfo>();
                progress.Subscribe(x =>
                {
                    if (task.Parent!._progress is Subject<ProgressInfo> parentProgress)
                        parentProgress.OnNext(new ProgressInfo
                        {
                            name = x.name,
                            progress = task.CalculateProgress(x.progress),
                            IsIndeterminate = x.IsIndeterminate
                        });
                    else
                        throw new InvalidOperationException(
                            "Parent progress cannot be other than Subject<ProgressInfo>");
                });
                task._progress = progress;
                Tasks.Add(task);
            }
            else if (value is List<string> list)
            {
                var task = new TaskProgress(this, list)
                {
                    Name = key
                };
                var progress = new Subject<ProgressInfo>();
                progress.Subscribe(x =>
                {
                    if (task.Parent!._progress is Subject<ProgressInfo> parentProgress)
                        parentProgress.OnNext(new ProgressInfo
                        {
                            name = x.name,
                            progress = task.CalculateProgress(x.progress),
                            IsIndeterminate = x.IsIndeterminate
                        });
                    else
                        throw new InvalidOperationException(
                            "Parent progress cannot be other than Subject<ProgressInfo>");
                });
                task._progress = progress;
                Tasks.Add(task);
            }
            else
            {
                Tasks.Add(new TaskProgress(this)
                {
                    Name = key
                });
            }
    }

    private void InitializeTasks(List<string> tasks)
    {
        foreach (var name in tasks)
            Tasks.Add(new TaskProgress(this)
            {
                Name = name
            });
    }

    public void NextTask(int step)
    {
        if (!IsRoot) throw new InvalidOperationException("This method can only be called from root task");
        for (var i = 0; i < step; i++) NextTaskRoot();
    }

    public void NextTask()
    {
        if (IsCompleted) throw new InvalidOperationException("This task is already completed");
        if (IsRoot)
        {
            NextTaskRoot();
        }
        else
        {
            NextTaskNode();
            Parent!.taskIdx += 1;
            Parent!.Task?.ActivateTasks();
        }
    }

    private bool NextTaskRoot()
    {
        if (IsLeafNode)
        {
            taskIdx += 1;
            return true;
        }

        if (Task!.NextTaskRoot())
        {
            taskIdx += 1;
            Task?.ActivateTasks();
            return IsCompleted;
        }

        return false;
    }

    private void ActivateTasks()
    {
        if (IsLeafNode) return;
        taskIdx = 0;
        Task!.ActivateTasks();
    }

    private void NextTaskNode()
    {
        if (IsLeafNode)
            taskIdx += 1;
        else
            do
            {
                Task!.NextTaskNode();
                taskIdx += 1;
                Task?.ActivateTasks();
            } while (taskIdx < Tasks.Count);
    }

    public void AddTask(string name)
    {
        if (IsLeafNode)
        {
            var progress = new Subject<ProgressInfo>();
            progress.Subscribe(x =>
            {
                if (Parent!._progress is Subject<ProgressInfo> parentProgress)
                    parentProgress.OnNext(new ProgressInfo
                    {
                        name = x.name,
                        progress = CalculateProgress(x.progress),
                        IsIndeterminate = x.IsIndeterminate
                    });
                else throw new InvalidOperationException("Parent progress cannot be other than Subject<ProgressInfo>");
            });
            _progress = progress;
        }

        Tasks.Add(new TaskProgress(this)
        {
            Name = name
        });
    }

    public void InsertTask(int index, string name)
    {
        if (IsLeafNode)
        {
            var progress = new Subject<ProgressInfo>();
            progress.Subscribe(x =>
            {
                if (Parent!._progress is Subject<ProgressInfo> parentProgress)
                    parentProgress.OnNext(new ProgressInfo
                    {
                        name = x.name,
                        progress = CalculateProgress(x.progress),
                        IsIndeterminate = x.IsIndeterminate
                    });
                else throw new InvalidOperationException("Parent progress cannot be other than Subject<ProgressInfo>");
            });
            _progress = progress;
        }

        Tasks.Insert(index, new TaskProgress(this)
        {
            Name = name
        });
    }

    public Subject<ProgressInfo> GetSubjectProgress()
    {
        if (IsLeafNode)
        {
            var progress = new Subject<ProgressInfo>();
            progress.Subscribe(x =>
            {
                if (Parent!._progress is Subject<ProgressInfo> parentProgress)
                    parentProgress.OnNext(new ProgressInfo
                    {
                        name = x.name,
                        progress = CalculateProgress(x.progress),
                        IsIndeterminate = x.IsIndeterminate
                    });
                else throw new InvalidOperationException("Parent progress cannot be other than Subject<ProgressInfo>");
            });
            _progress = progress;
            return progress;
        }

        return Task!.GetSubjectProgress();
    }

    public Progress<double> GetProgress()
    {
        if (IsLeafNode)
        {
            var progress = new Progress<double>();
            progress.ProgressChanged += HandleProgressChanged;
            _progress = progress;
            return progress;
        }

        return Task!.GetProgress();
    }

    private void HandleProgressChanged(object? sender, double d)
    {
        if (Parent!._progress is Subject<ProgressInfo> parentProgress)
            parentProgress.OnNext(new ProgressInfo
            {
                name = Name,
                progress = CalculateProgress(d)
            });
        else throw new InvalidOperationException("Parent progress cannot be other than Subject<ProgressInfo>");
    }

    private double CalculateProgress(double value)
    {
        return 1.0 / Parent!.Tasks.Count * Parent!.taskIdx + value / Parent!.Tasks.Count;
    }

    private void DisposeProgress()
    {
        if (IsRoot) return;
        if (_progress is Subject<ProgressInfo> progress1) progress1.Dispose();
        else if (_progress is Progress<double> progress2) progress2.ProgressChanged -= HandleProgressChanged;
    }
}