const express = require("express");

const app = express();
const port = 3000;

app.use(express.json());

app.get("/", (_, res) => {
  res.json({ status: "Webhook is running." });
});

app.post("/consumer", (req, res) => {
  const timestamp = new Date().toISOString();
  const body = req.body ?? null;

  console.log({
    timestamp,
    payload: body,
  });

  res.json({
    timestamp,
    payload: body,
  });
});

app.listen(port, () => {
  console.log(`WebHook listening on port ${port}`);
});
