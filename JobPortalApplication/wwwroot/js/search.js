document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("searchInput");
    const clearButton = document.createElement("span");
    clearButton.innerHTML = "&#x2715;";
    clearButton.style.cursor = "pointer";
    clearButton.style.position = "absolute";
    clearButton.style.right = "10px";
    clearButton.style.top = "50%";
    clearButton.style.transform = "translateY(-50%)";
    clearButton.style.display = "none";

    searchInput.parentElement.style.position = "relative";
    searchInput.parentElement.appendChild(clearButton);

    searchInput.addEventListener("input", function () {
        clearButton.style.display = searchInput.value ? "block" : "none";
    });

    clearButton.addEventListener("click", function () {
        searchInput.value = "";
        clearButton.style.display = "none";
        searchInput.focus();
    });

    document.getElementById("searchBtn").addEventListener("click", async function () {
        const searchTerm = searchInput.value;

        try {
            const response = await fetch(`/Job/SearchJobsByPosition?searchTerm=${encodeURIComponent(searchTerm)}`);
            const jobs = await response.json();

            const jobsListDiv = document.getElementById("jobsList");
            jobsListDiv.innerHTML = "";

            if (jobs.length === 0) {
                jobsListDiv.innerHTML = "<p>No jobs found for the given search term.</p>";
            } else {
                jobs.forEach(job => {
      
                    const postedTime = new Date(job.postedTime);
                    const formattedPostedTime = postedTime.toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: '2-digit'
                    }).replace(/ /g, '-');

                    const jobCard = `
                        <div class="col">
                            <div class="card shadow-sm">
                                <img class="card-img-top" src="${job.image}" alt="Card image cap" width="100%" height="225px">
                                <div class="card-body">
                                    <p class="card-text">${job.position}</p>
                                    <p class="card-text">${job.description}</p>
                                    <div class="d-flex justify-content-between align-items-center">
                                        <div class="btn-group">
                                            <a class="btn btn-sm btn-outline-secondary" href="/Job/Detail/${job.id}">View</a>
                                        </div>
                                        <small class="text-body-secondary">${formattedPostedTime}</small> 
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    jobsListDiv.innerHTML += jobCard;
                });
            }
        } catch (error) {
            console.error("Error fetching search results:", error);
        }

        searchInput.value = "";
        clearButton.style.display = "none";
    });
});
