using System;

namespace Symirror3.Rendering;

internal interface IFrameTask
{
    IMessage Message { get; }
    bool Invoke();
}

internal class FrameTask<T> : IFrameTask
    where T : IMessage
{
    private readonly Func<T, bool> _action;

    public T Message { get; }

    IMessage IFrameTask.Message => Message;

    internal FrameTask(T message, Func<T, bool> action) => (Message, _action) = (message, action);

    public bool Invoke() => _action(Message);
}

internal class CountFrameTask<T> : IFrameTask
    where T : IMessage
{
    private readonly Action<T, int> _action;
    private int _count;
    public T Message { get; }

    IMessage IFrameTask.Message => Message;

    internal CountFrameTask(T message, int count, Action<T, int> action) =>
        (Message, _count, _action) = (message, count, action);

    public bool Invoke()
    {
        _count--;
        _action(Message, _count);
        return _count <= 0;
    }
}
