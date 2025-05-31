// namespace BeerSender.Domain.Boxes.Handlers;
//
// class Close_box_handler : Command_handler<Close_box, Box>
// {
//     public Close_box_handler(
//         IEnumerable<Event> event_stream,
//         Action<Event> publish_event)
//         : base(event_stream, publish_event)
//     { }
//
//     protected override IEnumerable<Event> Handle_command(
//         Box aggregate,
//         Close_box command)
//     {
//         if (aggregate.Contents.Any())
//             yield return new Box_closed();
//         else
//             yield return new Box_failed_to_close(
//                 Box_failed_to_close.Fail_reason.Box_was_empty);
//     }
// }