// namespace BeerSender.Domain.Boxes.Handlers;
//
// class Ship_box_handler : Command_handler<Ship_box, Box>
// {
//     public Ship_box_handler(
//         IEnumerable<Event> event_stream,
//         Action<Event> publish_event)
//         : base(event_stream, publish_event)
//     { }
//
//     protected override IEnumerable<Event> Handle_command(
//         Box aggregate,
//         Ship_box command)
//     {
//         if (aggregate.Closed && aggregate.Shipping_label is not null)
//         {
//             yield return new Box_shipped();
//             yield break;
//         }
//
//         if (!aggregate.Closed)
//             yield return new Box_was_not_ready(
//                 Box_was_not_ready.Fail_reason.Box_was_not_closed);
//
//         if (aggregate.Shipping_label is null)
//             yield return new Box_was_not_ready(
//                 Box_was_not_ready.Fail_reason.Box_has_no_shipping_label);
//     }
// }