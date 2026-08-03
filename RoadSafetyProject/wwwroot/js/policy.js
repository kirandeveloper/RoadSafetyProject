document.addEventListener("DOMContentLoaded", function () {

    const policyContainer = document.getElementById("policyContainer");
    const policyCount = document.getElementById("policyCount");

    fetch("assets/policy.json")
        .then(response => {
            if (!response.ok) {
                throw new Error("Unable to load policy.json");
            }
            return response.json();
        })
        .then(policies => {

            // ==========================
            // Sort Latest to Oldest
            // ==========================
            policies.sort((a, b) => {

                // If JSON has proper date field
                if (a.date && b.date) {
                    return new Date(b.date) - new Date(a.date);
                }

                // Otherwise sort using day + month text
                const parseDate = (item) => {
                    const dateStr = `${item.day} ${item.month}`;
                    return new Date(dateStr);
                };

                return parseDate(b) - parseDate(a);

            });

            // ==========================
            // Dynamic Policy Count
            // ==========================
            const count = policies.length;

            if (policyCount) {
                policyCount.innerHTML =
                    `${count} ${count === 1 ? "Policy" : "Policies"} on Record`;
            }

            // ==========================
            // Generate Cards
            // ==========================
            let html = "";

            policies.forEach(item => {

                let day = item.day;
                let month = item.month;

                // If using new JSON date field
                if (item.date) {

                    const d = new Date(item.date);

                    day = d.getDate().toString().padStart(2, "0");

                    month = d.toLocaleString("en-US", {
                        month: "short",
                        year: "numeric"
                    });

                }

                html += `
                    <div class="policy-card">

                        <div class="policy-date">
                            <span>${day}</span>
                            <small>${month}</small>
                        </div>

                        <div class="policy-body">

                            <div class="policy-title">

                                ${item.title}

                                <a href="${item.file}"
                                   target="_blank"
                                   class="ms-2"
                                   title="View PDF">

                                    <img src="img/pdf.png"
                                         width="24"
                                         alt="PDF">

                                </a>

                            </div>

                        </div>

                    </div>
                `;

            });

            policyContainer.innerHTML = html;

        })
        .catch(error => {

            console.error(error);

            if (policyContainer) {

                policyContainer.innerHTML = `
                    <div class="alert alert-danger text-center">
                        <strong>Error!</strong><br>
                        Unable to load policy records.
                    </div>
                `;

            }

            if (policyCount) {
                policyCount.innerHTML = "0 Policies on Record";
            }

        });

});